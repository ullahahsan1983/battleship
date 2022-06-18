import React, { Component } from 'react';
import { Container, Row, Col, Table, Button, Badge, Toast, ToastContainer, Modal, Stack } from 'react-bootstrap';
import BattleGrid from './BattleGrid';
import env from './Environment';

function timeout(delay) {
    return new Promise(res => setTimeout(res, delay));
}

export default class Game extends Component {
    constructor(props) {
        super(props);

        this.heads = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];

        this.state = this.initialState();
    }

    initialState() {
        return {
            loading: true,
            started: false,
            grid1: [],
            grid2: [],
            gameState: 'Initiated',
            player: {},
            opponent: {},
            showResult: false,
            showCounter: false,
            winner: '',
            gameOver: false,
            showWinner: false
        };
    }

    componentDidMount() {
        this.startNewGame();
    }

    setShowResult = (show) => this.setState({ showResult: show });

    setShowCounter = (show) => this.setState({ showCounter: show });

    setShowWinner = (show) => this.setState({ showWinner: show });

    render() {
        const { grid1, grid2, player, opponent } = this.state;

        const legends = this.heads.map((item,i) => {
            return (<th key={i+1} className='legend-cell top'><Badge bg="dark">{item}</Badge></th>)
        });

        return (
            <>
                {this.state.started && this.props.show &&
                    <Container fluid>
                        <Row className='game'>
                            <Col lg={8} className='game-panel'>
                                <div className='battle-zone'>
                                    <Row>
                                        <Col>
                                            <BattleGrid locked={this.state.gameOver} grid={grid2} attack={(coord) => this.launchAttack(coord)} />
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Col>
                                            <Table className='legend' borderless size='sm'>
                                                <thead>
                                                    <tr>
                                                        {legends}
                                                    </tr>
                                                </thead>
                                            </Table>
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Col>
                                            <BattleGrid grid={grid1} player />
                                        </Col>
                                    </Row>
                                </div>
                            </Col>
                            <Col lg={4} className='side-panel'>
                                <Button variant="dark" onClick={() => this.startNewGame()}>{this.state.gameOver ? 'Start New Game' : 'Start Over'}</Button>{' '}
                            </Col>
                        </Row>

                        <ToastContainer position="top-end" className="p-3">
                            <Toast bg={env.Theme1} onClose={() => this.setShowResult(false)} show={this.state.showResult} delay={3000} autohide>
                                <Toast.Header>
                                    <strong className="me-auto">{env.Player} - {player.lastMove}</strong>
                                </Toast.Header>
                                <Toast.Body className='bg-light'>{player.lastResult === 'Missed' ? "Damn, I missed!" : "Woohoo, Bull's eye"}</Toast.Body>
                            </Toast>

                            <Toast bg={env.Theme2} onClose={() => this.setShowCounter(false)} show={this.state.showCounter} delay={3000} autohide>
                                <Toast.Header>
                                    <strong className="me-auto">{env.Opponent} - {opponent.lastMove}</strong>
                                </Toast.Header>
                                <Toast.Body className='bg-light'>{opponent.lastResult === 'Missed' ? "Miss" : "Hit!!!"}</Toast.Body>
                            </Toast>
                        </ToastContainer>                    
                    </Container>
                }
                <Modal centered show={this.state.showWinner} className='winner-modal' onHide={() => this.setShowWinner(false)} >
                    <Modal.Header closeButton>Winner</Modal.Header>
                    <Modal.Body>
                        <Stack direction="vertical" className="mx-auto">
                            <h1 className="mx-auto">{this.state.winner} Won</h1>
                        </Stack>
                        <Button variant='secondary' onClick={() => this.startNewGame()}>Play Again</Button>
                    </Modal.Body>
                </Modal>
            </>
        );
    }

    getPlacement(coord) {
        const head = this.heads[coord.colIndex + 1];
        return `${head}${coord.rowIndex + 1}`;
    }

    checkGameOver(report) {
        if (report.winner) {
            this.setState({ winner: report.winner, gameOver: true, showWinner: true });
            return true;
        }
        return false;
    }

    async startNewGame() {
        this.setState(this.initialState());

        await this.requestNewGame()
            .then(gameReport => {
                this.setState({
                    loading: false,
                    grid1: gameReport.grid1,
                    grid2: gameReport.grid2,
                    started: true
                });
            });        
    }

    async requestNewGame() {
        const response = await fetch(`${env.Api}/Battleship/StartNewGame`, {
            method: 'POST'
        });
        return response.json();
    }

    async launchAttack(coord) {
        this.setState({ loading: true });

        // a bit delay to show some spinning for heavy warfare :):)
        await timeout(1000);

        const response = await fetch(`${env.Api}/Battleship/Attack`, {
            method: 'POST',
            body: JSON.stringify(coord),
            headers: { 'Content-Type': 'application/json' },
        });
        const report = await response.json();

        const { target, counterTarget } = report;
        const { grid1, grid2 } = this.state;

        var attack = grid2[target.id - 1];
        var counter = grid1[counterTarget.id - 1];

        attack.isRevealed = target.isRevealed;
        attack.state = target.state;
        attack.busy = false;

        this.setState({
            gameState: report.gameState,
            player: { lastMove: this.getPlacement(target.coordinate), lastResult: report.result },
            showResult: true
        });

        if (this.checkGameOver(report)) return;

        counter.busy = true;
        // a bit delay to emulate counter warfare :):)
        await timeout(2000);

        counter.isRevealed = counterTarget.isRevealed;
        counter.state = counterTarget.state;
        counter.busy = false;

        this.setState({
            opponent: { lastMove: this.getPlacement(counterTarget.coordinate), lastResult: report.counterResult },
            showCounter: true,
            loading: false
        });

        this.checkGameOver(report);
    }
}