import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'
import { ResourcesBox } from './ResourcesBox';
import { UpgradeBox } from './UpgradeBox';

export const DomikiPage = () => {
    const [domiks, setDomiks] = useState({});
    const [domikTypes, setDomikTypes] = useState([]);
    const [resources, setResources] = useState([]);
    const [resourceTypes, setResourceTypes] = useState([]);
    const [purchaseDomikTypes, setPurchaseDomikTypes] = useState([]);
    const [purchaseDomikTypesVisible, setPurchaseDomikTypesVisible] = useState([]);
    const [modificatorTypes, setModificatorTypes] = useState([]);
    const [plodderCount, setPlodderCount] = useState(null);

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
            sendRequest('GET', 'Domiki/GetModificatorTypes', function (data) {
                setModificatorTypes(data);
            });
            getDomiks();
            getResources();
        }

        myFunc();

    }, []);


    useEffect(() => {
        const interval = setInterval(function () {
            var result = IntervalTick(domiks.items);
            if (result) {
                setDomiks({ items: domiks.items });
            }
        }, 1000);

        return () => {
            if (interval != null) {
                clearInterval(interval);
            }
        };

    }, [domiks]);

    function IntervalTick(domikItems) {
        if (domikItems != null) {
            domikItems.forEach(function (domik) {
                if (domik.finishDate != null) {
                    let date = new Date();
                    let seconds = (new Date(domik.finishDate).getTime() - date.getTime()) / 1000;
                    let time = getTimeFromSecond(seconds);
                    domik.upgradeSeconds = time;
                    if (seconds <= 0) {
                        getDomiks();
                        return false;
                    }
                }
            })
        }
        return true;
    }

    function getTimeFromSecond(totalSeconds) {
        totalSeconds = Math.round(totalSeconds, 0);
        var seconds = totalSeconds % 60;
        var minuts = parseInt(totalSeconds / 60);
        var hours = 0;
        var days = 0;
        if (minuts > 0) {
            hours = parseInt(minuts / 60);
            minuts = minuts % 60;
        }
        if (hours > 0) {
            days = parseInt(hours / 24);
            hours = hours % 24;
        }
        var showInfo = "";
        if (days > 0) {
            if (days < 10) {
                days = '0' + days;
            }
            showInfo += days + "д ";
        }
        if (hours > 0 || days > 0) {
            if (hours < 10) {
                hours = '0' + hours;
            }
            showInfo += hours + "ч ";
        }
        if (minuts > 0 || days > 0 || hours > 0) {
            if (minuts < 10) {
                minuts = '0' + minuts;
            }
            showInfo += minuts + "м ";
        }
        if (days === 0) {
            if (seconds < 10) {
                seconds = '0' + seconds;
            }
            showInfo += seconds + "с ";
        }
        return showInfo;

    }


    async function getDomiks() {
        sendRequest('GET', 'Domiki/GetDomiks', function (data) {
            IntervalTick(data)
            setDomiks({ items: data });

            let plodderCount = 0;
            // todo а если типов домиков ещё не получили, то сосём бибу
            if (domikTypes != null && domikTypes.length > 0 && data != null) {
                data.forEach(function (domik) {
                    let domikType = domikTypes.filter(x => x.id === domik.typeId)[0];
                    if (domik.level > 0) {
                        let domiklevel = domikType.levels.filter(x => x.value === domik.level)[0];
                        let plodderTypeId = 1;
                        let modificators = domiklevel.modificators.filter(x => x.typeId === plodderTypeId);
                        if (modificators.length > 0) {
                            plodderCount += modificators[0].value;
                        }
                    }
                });
            }
            setPlodderCount(plodderCount);
        });
    }

    async function getResources() {
        sendRequest('GET', 'Domiki/GetResources', function (data) {
            setResources(data);
        });
    }

    async function upgrade(id) {
        sendRequest('POST', 'Domiki/UpgradeDomik/' + id, function (data) {
            getDomiks();
            getResources();
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
            getResources();
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
                if (data.type === 2) {
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
            <div>
                {plodderCount != null &&
                    <div>
                        <img src="/images/modificatorTypes/plodder.png" alt="Трудяги"></img>
                        <label>{plodderCount}</label>
                    </div>
                }
            </div>
            <div className="resources">
                {resourceTypes != null && resourceTypes.length > 0 &&
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
                {domikTypes != null && domikTypes.length > 0 && domiks.items != null &&
                    domiks.items.map((domik, index) => {
                        let domikType = domikTypes.filter(x => x.id === domik.typeId)[0];
                        let image = "/images/domikTypes/" + domikType.logicName + ".png";
                        return (
                            <div key={index} className="domik-box">
                                <img src={image} alt={domikType.name} />
                                <div className="break" />
                                <UpgradeBox upgradeSeconds={domik.upgradeSeconds} level={domik.level} />
                                <div className="break" />
                                {domik.level < domikType.maxLevel &&
                                    <button onClick={() => upgrade(domik.id)}>улучшить</button>
                                    /* todo после улучшения не показывается время, нужно жмакать Ф5 */
                                    /* todo если домик улучшается, то прятать кнопочку */
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
                                    <ResourcesBox resources={purchaseDomikType.levels[0].resources} resourceTypes={resourceTypes} />
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