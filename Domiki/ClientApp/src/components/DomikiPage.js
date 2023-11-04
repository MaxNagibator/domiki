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
            const token = await authService.getHeaderWithAccessToken();
            let param = {
                headers: token
            }
            fetch('https://localhost:7146/Domiki/GetDomikTypes', param)
                .then((res) => res.json())
                .then((data) => {
                    setDomikTypes(data);
                })
                .catch((err) => {
                    console.log(err.message);
                });
            fetch('https://localhost:7146/Domiki/GetResourceTypes', param)
                .then((res) => res.json())
                .then((data) => {
                    setResourceTypes(data);
                })
                .catch((err) => {
                    console.log(err.message);
                });
            getDomiks();
            getResources();
        }

        myFunc();
    }, []);

    async function getDomiks() {
        const token = await authService.getHeaderWithAccessToken();
        let param = {
            headers: token
        }
        fetch('https://localhost:7146/Domiki/GetDomiks', param)
            .then((res) => res.json())
            .then((data) => {
                setDomiks(data);
            })
            .catch((err) => {
                console.log(err.message);
            });
    }

    async function getResources() {
        const token = await authService.getHeaderWithAccessToken();
        let param = {
            headers: token
        }
        fetch('https://localhost:7146/Domiki/GetResources', param)
            .then((res) => res.json())
            .then((data) => {
                setResources(data);
            })
            .catch((err) => {
                console.log(err.message);
            });
    }

    async function upgrade(id) {
        const token = await authService.getAccessToken();
        const requestOptions = {
            method: 'POST',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        };
        fetch('https://localhost:7146/Domiki/UpgradeDomik/' + id, requestOptions)
            .then((res) => res.json())
            .then((data) => {
                if (data.Type === 2) {
                    alert(data.Content);
                } else {
                    getDomiks();
                }
            })
            .catch((err) => {
                console.log(err.message);
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
        const token = await authService.getAccessToken();
        const requestOptions = {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        };
        fetch('https://localhost:7146/Domiki/GetPurchaseAvaialableDomiks', requestOptions)
            .then((res) => res.json())
            .then((data) => {
                setPurchaseDomikTypes(data);
            })
            .catch((err) => {
                console.log(err.message);
            });
    }

    async function buy(typeId) {
        const token = await authService.getAccessToken();
        const requestOptions = {
            method: 'POST',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        };
        fetch('https://localhost:7146/Domiki/BuyDomik/' + typeId, requestOptions)
            .then((res) => res.json())
            .then((data) => {
                if (data.Type === 2) {
                    alert(data.Content);
                } else {
                    getDomiks();
                    getPurchaseDomikTypes();
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