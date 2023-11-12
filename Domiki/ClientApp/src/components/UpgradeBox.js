import React from 'react';

export const UpgradeBox = ({ upgradeSeconds, level }) => {
    console.log(upgradeSeconds);
    let levelText = upgradeSeconds == null ? level : level + " -> " + (level * 1 + 1);

    return (
        <div className="upgrade-box">
            <div>
                <label className="domik-level">{levelText}</label>
            </div>
            <div className="break" />
            {upgradeSeconds != null &&
                <div>
                    <label>{upgradeSeconds}</label>
                    <div className="break" />
                </div>
            }
        </div>
    );
};