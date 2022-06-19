import React, { useState } from 'react';
import { Modal, Button, Stack } from 'react-bootstrap';
import './App.css';
import Layout from './components/Layout/Layout';
import About from './components/About/About';
import Game from './components/Game/Game';
import env from './components/Game/Environment';
import ship from './components/Game/ship-wreck.png';

const App = () => {
    const [show, setShow] = useState(false);

    return (
        <Layout>
            <Modal centered show={!show} className='welcome-modal'>
                <Modal.Body>
                    <Stack direction="vertical" className="mx-auto">
                        <h1 className="mx-auto">{env.Title}</h1>
                        <h6 className="mx-auto">{env.Subtitle}</h6>
                        <img src={ship} className='mx-auto img-fluid' />
                    </Stack>
                    <Button variant='outline-info' onClick={() => setShow(true)}>Play</Button>
                </Modal.Body>
            </Modal>
            <About />
            <Game show={show} />
        </Layout>
    );
}

export default App;
