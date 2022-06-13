import React from 'react';
import './App.css';
import Layout from './components/Layout/Layout';
import Game from './components/Game/Game';

const app = () => {
    return (
        <Layout>
            <Game />
        </Layout>
    );
}

export default app;
