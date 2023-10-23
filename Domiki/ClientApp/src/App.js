import './App.css';
import React, { useState, useEffect } from 'react';

function App() {
    const [domiks, setDomiks] = useState([]);
    const [domikTypes, setDomikTypes] = useState([]);


    useEffect(() => {
        fetch('https://localhost:7146/Domiki/GetDomikTypes')
            .then((res) => res.json())
            .then((data) => {
                setDomikTypes(data);
            })
            .catch((err) => {
                console.log(err.message);
            });
    }, []);

    useEffect(() => {
        fetch('https://localhost:7146/Domiki/GetDomiks')
            .then((res) => res.json())
            .then((data) => {
                setDomiks(data);
            })
            .catch((err) => {
                console.log(err.message);
            });
    }, []);
    return (
        <div className="App">
            {domiks != null && domikTypes != null &&
                domiks.map((domik, index) => {
                    let domikType = domikTypes.filter(x => x.id === domik.typeId)[0];
                    let image = "/images/domikTypes/" + domikType.logicName + ".png";
                    return (
                        <div key={index} className="DomikBox">
                            <img src={image} alt={domikType.name} />
                            <label>level: {domik.level}</label>
                        </div>
                    );
                })}
        </div>
    );
}

export default App;
