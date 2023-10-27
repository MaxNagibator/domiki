import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'

export const DomikiPage = () => {
    const[domiks, setDomiks] = useState([]);
    const[domikTypes, setDomikTypes] = useState([]);
    useEffect(() => {

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
            //headers: { 'Content-Type': 'application/json' },
            //body: JSON.stringify({ id: id })
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

    return (
        //<div>
        //    Hello bomsh
        //</div>
        <div className="App">
            {domiks != null && domikTypes != null &&
                domiks.map((domik, index) => {
                    let domikType = domikTypes.filter(x => x.id === domik.typeId)[0];
                    let image = "/images/domikTypes/" + domikType.logicName + ".png";
                    return (
                        <div key={index} className="DomikBox">
                            <img src={image} alt={domikType.name} />
                            <label>level: {domik.level}</label>
                            <button onClick={() => handleClick(domik.id)}>lvl up</button>
                        </div>
                    );
                })}
            </div>
    );
};


//

//export class Home extends Component {
//    /*  static displayName = Home.name;*/

//    //const[domiks, setDomiks] = useState([]);
//    //const[domikTypes, setDomikTypes] = useState([]);

//    handleClick = function (id) {
//        const requestOptions = {
//            method: 'POST'
//            //headers: { 'Content-Type': 'application/json' },
//            //body: JSON.stringify({ id: id })
//        };
//        fetch('https://localhost:7146/Domiki/UpgradeDomik/' + id, requestOptions)
//            .then((res) => res.json())
//            .then((data) => {
//                //setDomiks(data);
//            })
//            .catch((err) => {
//                console.log(err.message);
//            });
//    }

//    componentDidMount() {
//        fetch('https://localhost:7146/Domiki/GetDomikTypes')
//            .then((res) => res.json())
//            .then((data) => {
//                setDomikTypes(data);
//            })
//            .catch((err) => {
//                console.log(err.message);
//            });
//        fetch('https://localhost:7146/Domiki/GetDomiks')
//            .then((res) => res.json())
//            .then((data) => {
//                setDomiks(data);
//            })
//            .catch((err) => {
//                console.log(err.message);
//            });
//    }

//    //useEffect(() => {
//    //}, []);

//    //useEffect(() => {

//    //}, []);

//    render() {
//        return (
//            <div className="App">
//                {domiks != null && domikTypes != null &&
//                    domiks.map((domik, index) => {
//                        let domikType = domikTypes.filter(x => x.id === domik.typeId)[0];
//                        let image = "/images/domikTypes/" + domikType.logicName + ".png";
//                        return (
//                            <div key={index} className="DomikBox">
//                                <img src={image} alt={domikType.name} />
//                                <label>level: {domik.level}</label>
//                                <button onClick={() => handleClick(domik.id)}>lvl up</button>
//                            </div>
//                        );
//                    })}
//            </div>
//        );
//    }
//}
//import React, { Component } from 'react';

//export class FetchData extends Component {
//  static displayName = FetchData.name;

//  constructor(props) {
//    super(props);
//    this.state = { forecasts: [], loading: true };
//  }

//  componentDidMount() {
//    this.populateWeatherData();
//  }

//  static renderForecastsTable(forecasts) {
//    return (
//      <table className='table table-striped' aria-labelledby="tabelLabel">
//        <thead>
//          <tr>
//            <th>Date</th>
//            <th>Temp. (C)</th>
//            <th>Temp. (F)</th>
//            <th>Summary</th>
//          </tr>
//        </thead>
//        <tbody>
//          {forecasts.map(forecast =>
//            <tr key={forecast.date}>
//              <td>{forecast.date}</td>
//              <td>{forecast.temperatureC}</td>
//              <td>{forecast.temperatureF}</td>
//              <td>{forecast.summary}</td>
//            </tr>
//          )}
//        </tbody>
//      </table>
//    );
//  }

//  render() {
//    let contents = this.state.loading
//      ? <p><em>Loading...</em></p>
//      : FetchData.renderForecastsTable(this.state.forecasts);

//    return (
//      <div>
//        <h1 id="tabelLabel" >Weather forecast</h1>
//        <p>This component demonstrates fetching data from the server.</p>
//        {contents}
//      </div>
//    );
//  }

//  async populateWeatherData() {
//    const response = await fetch('weatherforecast');
//    const data = await response.json();
//    this.setState({ forecasts: data, loading: false });
//  }
//}
