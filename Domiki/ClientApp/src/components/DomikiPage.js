import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'

export const DomikiPage = () => {
    const[domiks, setDomiks] = useState([]);
    const [domikTypes, setDomikTypes] = useState([]);
    const [purchaseDomikTypes, setPurchaseDomikTypes] = useState([]);
    useEffect(() => {
        setPurchaseDomikTypes(null);
        async function myFunc() {
            const token = await authService.getAccessToken();
            let param = {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            }
            fetch('https://localhost:7146/Domiki/GetDomikTypes', param)
                .then((res) => res.json())
                .then((data) => {
                    setDomikTypes(data);
                })
                .catch((err) => {
                    console.log(err.message);
                });
            fetch('https://localhost:7146/Domiki/GetDomiks', param)
                .then((res) => res.json())
                .then((data) => {
                    setDomiks(data);
                })
                .catch((err) => {
                    console.log(err.message);
                });
        }

        myFunc();
    }, []);

    async function handleClick(id) {
        const token = await authService.getAccessToken();
        const requestOptions = {
            method: 'POST',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        };
        fetch('https://localhost:7146/Domiki/UpgradeDomik/' + id, requestOptions)
            .then((res) => res.json())
            .then((data) => {
                //setDomiks(data);
            })
            .catch((err) => {
                console.log(err.message);
            });
    }

    async function handleClick(id) {
        const token = await authService.getAccessToken();
        const requestOptions = {
            method: 'POST',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        };
        fetch('https://localhost:7146/Domiki/UpgradeDomik/' + id, requestOptions)
            .then((res) => res.json())
            .then((data) => {
                //setDomiks(data);
            })
            .catch((err) => {
                console.log(err.message);
            });
    }

    async function showPurchaseDomikWindow() {
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
                //setPurchaseDomikTypes(data);
            })
            .catch((err) => {
                console.log(err.message);
            });
    }

    return (
        //<div>
        //    Hello bomsh
        //</div>
        <div className="App">
            <div className="Domiks">
            {domiks != null && domikTypes != null &&
                domiks.map((domik, index) => {
                    let domikType = domikTypes.filter(x => x.id === domik.typeId)[0];
                    let image = "/images/domikTypes/" + domikType.logicName + ".png";
                    return (
                        <div key={index} className="DomikBox">
                            <img src={image} alt={domikType.name} />
                            <label>level: {domik.level}</label>
                            <button onClick={() => handleClick(domik.id)}>улучшить</button>
                        </div>
                    );
                })
            }
            </div>
            <button onClick={() => showPurchaseDomikWindow()}> Купить домик</button>
            {purchaseDomikTypes != null &&
                purchaseDomikTypes.map((purchaseDomikType, index) => {
                    let image = "/images/domikTypes/" + purchaseDomikType.logicName + ".png";
                    return (
                        <div key={index} className="DomikBox">
                            <img src={image} alt={purchaseDomikType.name} />
                            <button onClick={() => buy(purchaseDomikType.id)}>купить</button>
                        </div>
                    );
                })
            }
        </div>
    );
};