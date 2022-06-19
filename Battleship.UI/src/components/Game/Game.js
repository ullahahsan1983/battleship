import React, { Component } from 'react';
import {
    Container, Row, Col, Table,
    Button, Badge, Toast, ToastContainer,
    Modal, Stack, Card, ListGroup
} from 'react-bootstrap';
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
            player: { vessels: ['intact','intact','intact'] },
            opponent: { vessels: ['intact', 'intact', 'intact'] },
            showResult: false,
            showCounter: false,
            victory: null,
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

    renderStats(isOpponent) {
        const getState = (vessel) => vessel === 'destroyed' ? 'danger' : (vessel === 'damaged' ? 'warning' : 'success');
        const theme = isOpponent ? env.Theme2 : env.Theme1;
        const label = isOpponent ? env.Opponent : env.Player;
        const vessels = isOpponent ? this.state.opponent.vessels : this.state.player.vessels;

        const stats = vessels.map((item, i) => {
            return (<ListGroup.Item key={i}>Vessel{i + 1} <Badge bg={getState(item)}>{item}</Badge></ListGroup.Item>);
        });

        return (
            <Card border={theme} bg={theme} text='light'>
                <Card.Header>{label}</Card.Header>
                <ListGroup variant="flush">
                    { stats }
                </ListGroup>
            </Card>
        );
    }

    render() {
        const { grid1, grid2, player, opponent, victory } = this.state;

        const winnerWindowClass = `ms-auto ${victory ? 'text-success' : 'text-danger'}`;

        const legends = this.heads.map((item,i) => {
            return (<th key={i + 1} className='legend-cell top'><Badge bg="dark">{item}</Badge></th>);
        });

        return (
            <>
                {this.state.started && this.props.show &&
                    <Container fluid className='game'>
                        <Row className='game-panel'>
                            <Col>
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
                        </Row>
                        <Row className='info-panel'>
                            <Col sm={1}></Col>
                            <Col sm={4}>
                                {this.renderStats(false)}
                            </Col>
                            <Col sm={3}>
                                <Stack direction="vertical" gap={2}>
                                    <Button className='mx-auto' variant="outline-dark" onClick={() => this.startNewGame()}>{this.state.gameOver ? 'Start New Game' : 'Start Over'}</Button>{' '}
                                    {this.state.gameOver && <Button className='mx-auto' variant="outline-dark" onClick={() => this.setShowWinner(true)}>Result</Button>}
                                </Stack>
                            </Col>
                            <Col sm={4}>
                                {this.renderStats(true)}
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
                    <Modal.Header closeButton>
                        <Modal.Title className={winnerWindowClass}>{ this.state.victory ? 'Victory!!' : 'Defeat' }</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Stack direction="vertical" gap={3} className="mx-auto">
                            <h1 className="mx-auto">{ this.state.victory ? env.Player : env.Opponent } Won</h1>
                            <Button className='ms-auto' variant='outline-secondary' onClick={() => this.startNewGame()}>Play Again</Button>
                        </Stack>
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
            this.setState({ victory: report.winner === 1, gameOver: true, showWinner: true });
            return true;
        }
        return false;
    }

    async startNewGame() {
        this.setState(this.initialState());

        const response = await fetch(`${env.Api}/Battleship/StartNewGame`, {
            method: 'POST'
        });

        await response.json()
            .then(gameReport => {
                this.setState({
                    loading: false,
                    grid1: gameReport.grid1,
                    grid2: gameReport.grid2,
                    started: true
                });
            });        
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

        const { grid1, grid2, player, opponent } = this.state;
        const { result, counterResult } = report;

        const target = result.target;
        const counterTarget = counterResult && counterResult.target;

        var targetCell = grid2[target.id - 1];
        targetCell.isRevealed = target.isRevealed;
        targetCell.state = target.state;
        targetCell.busy = false;

        player.lastMove = this.getPlacement(targetCell.coordinate);
        player.lastResult = result.state;
        if (result.affectedVessel) {
            opponent.vessels[result.affectedVessel - 1] = targetCell.state.toLowerCase();
        }

        this.setState({
            gameState: report.gameState,
            showResult: true
        });

        if (counterTarget) {
            var counterCell = grid1[counterTarget.id - 1];
            counterCell.busy = true;

            // a bit delay to emulate counter warfare :):)
            await timeout(2000);

            counterCell.isRevealed = counterTarget.isRevealed;
            counterCell.state = counterTarget.state;
            counterCell.busy = false;

            opponent.lastMove = this.getPlacement(counterCell.coordinate);
            opponent.lastResult = counterResult.state;
            if (counterResult.affectedVessel) {
                player.vessels[counterResult.affectedVessel - 1] = counterCell.state.toLowerCase();
            }

            this.setState({
                showCounter: true,
                loading: false
            });
        }

        this.checkGameOver(report);
    }
}