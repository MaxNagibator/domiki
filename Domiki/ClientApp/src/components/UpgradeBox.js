import React, { useState, useEffect } from 'react';

export const UpgradeBox = ({ finishDate, level }) => {
    let levelText = finishDate == null ? level : level + " -> " + (level * 1 + 1);
    const [seconds, setSeconds] = useState(null);

    useEffect(() => {
        GetSeconds();
        const interval = setInterval(function () {
            GetSeconds();
        }, 1000);
        return () => {
            clearInterval(interval);
        };
    }, []);


    async function GetSeconds() {
        if (finishDate != null) {
            let date = new Date();
            let seconds = (new Date(finishDate).getTime() - date.getTime()) / 1000;
            let time = getTimeFromSecond(seconds);
            setSeconds(await time);
        }
    }

    async function getTimeFromSecond(totalSeconds) {
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

    return (
        <div className="upgrade-box">
            <div>
            <label className="domik-level">{levelText}</label>
            </div>
            <div className="break" />
            {finishDate != null &&
                <div>
                    <label>{seconds}</label>
                    <div className="break" />
                </div>
            }
        </div>
    );
};