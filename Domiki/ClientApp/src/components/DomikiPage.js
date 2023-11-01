import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'

export const DomikiPage = () => {
    const [domiks, setDomiks] = useState([]);
    const [domikTypes, setDomikTypes] = useState([]);
    const [purchaseDomikTypes, setPurchaseDomikTypes] = useState([]);
    const [purchaseDomikTypesVisible, setPurchaseDomikTypesVisible] = useState([]);
    useEffect(() => {
        setPurchaseDomikTypes(null);
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
            getDomiks();
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

    async function upgrade(id) {
        const token = await authService.getAccessToken();
        const requestOptions = {
            method: 'POST',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        };
        fetch('https://localhost:7146/Domiki/UpgradeDomik/' + id, requestOptions)
            .then((res) => res.json())
            .then((data) => {
                getDomiks();
            })
            .catch((err) => {
                getDomiks(); // todo почему то считает пустой ответ ошибочным (доработать бэкэнд и возвращать чтото типо { status: success, data: bla bla})
                console.log(err.message);
            });
    }

    async function showPurchaseDomikWindow() {
        if (purchaseDomikTypesVisible === true) {
            setPurchaseDomikTypesVisible(false);
        } else {
            if (purchaseDomikTypes == null) {
                const token = await authService.getAccessToken();
                const requestOptions = {
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
                };
                fetch('https://localhost:7146/Domiki/GetPurchaseAvaialableDomiks', requestOptions)
                    .then((res) => res.json())
                    .then((data) => {
                        setPurchaseDomikTypes(data);
                        setPurchaseDomikTypesVisible(true);
                    })
                    .catch((err) => {
                        console.log(err.message);
                    });
            } else {
                setPurchaseDomikTypesVisible(true);
            }
        }
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
                getDomiks();
            })
            .catch((err) => {
                getDomiks(); // todo почему то считает пустой ответ ошибочным (доработать бэкэнд и возвращать чтото типо { status: success, data: bla bla})
                console.log(err.message);
            });
    }

    return (
        //<div>
        //    Hello bomsh
        //</div>
        <div className="App">
            <div className="domiks">
                {domiks != null && domikTypes != null &&
                    domiks.map((domik, index) => {

                        console.log('domikTypes');
                        console.log(domikTypes);
                        let domikType = domikTypes.filter(x => x.id === domik.typeId)[0];
                        console.log('domikType');
                        console.log(domikType);
                        let image = "/images/domikTypes/" + domikType.logicName + ".png";
                        return (
                            <div key={index} className="domik-box">
                                <img src={image} alt={domikType.name} />
                                <label className="domik-level">{domik.level}</label>
                                <button onClick={() => upgrade(domik.id)}>улучшить</button>
                            </div>
                        );
                    })
                }
            </div>

            <div className="purchase-box">
                <button onClick={() => showPurchaseDomikWindow()}> Купить домик</button>
                {purchaseDomikTypes != null && purchaseDomikTypesVisible === true &&
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
        </div>
    );
};