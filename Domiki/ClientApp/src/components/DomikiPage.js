import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'

export const DomikiPage = () => {
    const [domiks, setDomiks] = useState([]);
    const [domikTypes, setDomikTypes] = useState([]);
    const [resources, setResources] = useState([]);
    const [resourceTypes, setResourceTypes] = useState([]);
    const [purchaseDomikTypes, setPurchaseDomikTypes] = useState([]);
    const [purchaseDomikTypesVisible, setPurchaseDomikTypesVisible] = useState([]);

    useEffect(() => {
        setPurchaseDomikTypes(null);
        getPurchaseDomikTypes();
        async function myFunc() {
            sendRequest('GET', 'Domiki/GetDomikTypes', function (data) {
                setDomikTypes(data);
            });
            sendRequest('GET', 'Domiki/GetResourceTypes', function (data) {
                setResourceTypes(data);
            });
            getDomiks();
            getResources();
        }

        myFunc();
    }, []);

    async function getDomiks() {
        sendRequest('GET', 'Domiki/GetDomiks', function (data) {
            setDomiks(data);
        });
    }

    async function getResources() {
        sendRequest('GET', 'Domiki/GetResources', function (data) {
            setResources(data);
        });
    }

    async function upgrade(id) {
        sendRequest('POST', 'Domiki/UpgradeDomik/'+ id, function (data) {
            getDomiks();
            setResources();
        });
    }

    async function showPurchaseDomikWindow() {
        if (purchaseDomikTypesVisible === true) {
            setPurchaseDomikTypesVisible(false);
        } else {
            setPurchaseDomikTypesVisible(true);
            if (purchaseDomikTypes == null) {
                getPurchaseDomikTypes();
            }
        }
    }

    async function getPurchaseDomikTypes() {
        sendRequest('GET', 'Domiki/GetPurchaseAvaialableDomiks', function (data) {
            setPurchaseDomikTypes(data);
        });
    }

    async function buy(typeId) {
        sendRequest('POST', 'Domiki/BuyDomik/' + typeId, function (data) {
            getDomiks();
            setResources();
            getPurchaseDomikTypes();
        });
    }

    // todo переместить в сервис какойнить
    async function sendRequest(method, url, succesAction) {
        const token = await authService.getAccessToken();
        const requestOptions = {
            method: method,
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        };
        fetch('https://localhost:7146/' + url, requestOptions)
            .then((res) => res.json())
            .then((data) => {
                if (data.Type === 2) {
                    alert(data.content);
                } else {
                    succesAction(data.content);
                }
            })
            .catch((err) => {
                console.log(err.message);
            });
    }

    return (
        <div className="App">
            <div className="resources">
                {resources != null && resourceTypes != null &&
                    resources.map((resource, index) => {
                        let resourceType = resourceTypes.filter(x => x.id === resource.typeId)[0];
                        let image = "/images/resourceTypes/" + resourceType.logicName + ".png";
                        return (
                            <div key={index} className="resource-box">
                                <img src={image} alt={resourceType.name} />
                                <label className="resource-value">{resource.value}</label>
                            </div>
                        );
                    })
                }</div>
            <div className="domiks">

                {domiks != null && domikTypes != null &&
                    domiks.map((domik, index) => {
                        let domikType = domikTypes.filter(x => x.id === domik.typeId)[0];
                        let image = "/images/domikTypes/" + domikType.logicName + ".png";
                        return (
                            <div key={index} className="domik-box">
                                <img src={image} alt={domikType.name} />
                                <label className="domik-level">{domik.level}</label>
                                {domik.level < domikType.maxLevel &&
                                    <button onClick={() => upgrade(domik.id)}>улучшить</button>
                                }
                            </div>
                        );
                    })
                }
            </div>
            {purchaseDomikTypes != null && purchaseDomikTypes.length > 0 &&
                <div className="purchase-box">
                    <button onClick={() => showPurchaseDomikWindow()}>Магазин</button>
                    {purchaseDomikTypesVisible === true &&
                        purchaseDomikTypes.map((purchaseDomikType, index) => {
                            let image = "/images/domikTypes/" + purchaseDomikType.logicName + ".png";
                            return (
                                <div key={index} className="domik-box">
                                    <img src={image} alt={purchaseDomikType.name} />
                                    <button onClick={() => buy(purchaseDomikType.id)}>купить</button>
                                </div>
                            );
                        })
                    }
                </div>
            }
        </div>
    );
};