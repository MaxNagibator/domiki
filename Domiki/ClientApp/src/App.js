import logo from './logo.svg';
import './App.css';
import React, { useState, useEffect } from 'react';

function App() {

    const [count, setCount] = useState(0);
    const [posts, setPosts] = useState([]);


    useEffect(() => {
        fetch('https://localhost:7146/WeatherForecast')
            .then((res) => res.json())
            .then((data) => {
                let asd = data[0].summary + data[1].summary + data[2].summary;
                setPosts(asd);
            })
            .catch((err) => {
                console.log(err.message);
            });
    }, []);

    return (
        <div className="App">
            <p>You posts {posts}</p>
            <p>You clicked {count} times</p>
            <header className="App-header">
                <div>
                    <p>You clicked {count} times</p>
                    <button onClick={() => setCount(count + 1)}>
                        Click me
                    </button>
                </div>
                <img src={logo} className="App-logo" alt="logo" />
                <p>
                    Edit <code>src/App.js</code> and save to reload.
                </p>
                <a
                    className="App-link"
                    href="https://reactjs.org"
                    target="_blank"
                    rel="noopener noreferrer"
                >
                    Learn React

                    <br></br>
                    <label>URA</label>
                </a>
            </header>
        </div>
    );
}

export default App;
