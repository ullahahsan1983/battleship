import React, { Component } from 'react';
import { Container, Row, Col, Table, Button, Badge } from 'react-bootstrap';
import BattleGrid from './BattleGrid';

export const ApiUrl = 'http://localhost:49593';

export default class Game extends Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: true,
            started: false,
            grid1: [],
            grid2: [],
            gameState: 'Initiated'
        };
    }

    render() {
        const { grid1, grid2 } = this.state;

        const legends = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'].map((item,i) => {
            return (<th key={i+1} className='legend-cell top'><Badge bg="dark">{item}</Badge></th>)
        });

        return (
            <Container className='game'>
                <Row className='action-buttons'>
                    <Col>
                        <Button variant="dark" onClick={() => this.startNewGame()}>Start New Battle</Button>{' '}
                    </Col>
                </Row>
                {this.state.started &&
                    <div className='battle-zone'>
                        <Row>
                            <Col>
                                <Table className='legend' borderless size='sm'>
                                    <thead>
                                        <tr>
                                            <th key={1} className='legend-cell top'>&nbsp;</th>
                                            {legends}
                                        </tr>
                                    </thead>
                                </Table>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <BattleGrid grid={grid2} attack={(coord) => this.launchAttack(coord)} />
                                <BattleGrid grid={grid1} player />
                            </Col>
                        </Row>
                    </div>
                }
            </Container>
        );
    }

    async startNewGame() {
        this.setState({ loading: true });

        const response = await fetch(`${ApiUrl}/Battleship/StartNewGame`, {
            method: 'POST'
        });
        const gameReport = await response.json();

        this.setState({
            loading: false,
            grid1: gameReport.player1Grid,
            grid2: gameReport.player2Grid,
            started: true
        });
    }

    async launchAttack(coord) {
        this.setState({ loading: true });

        const response = await fetch(`${ApiUrl}/Battleship/Attack`, {
            method: 'POST',
            body: JSON.stringify(coord),
            headers: { 'Content-Type': 'application/json' },
        });
        const attackReport = await response.json();

        const { target, counterTarget } = attackReport;
        const { grid1, grid2 } = this.state;

        var computer = grid2[target.id - 1];
        if (computer) {
            computer.isRevealed = target.isRevealed;
            computer.state = target.state;
            grid2[target.id - 1] = computer;
        }

        var player = grid1[counterTarget.id - 1];
        if (player) {
            player.isRevealed = counterTarget.isRevealed;
            player.state = counterTarget.state;
            grid1[counterTarget.id - 1] = player;
        }

        this.setState({
            loading: false,
            gameState: attackReport.gameState,
            grid1: grid1,
            grid2: grid2
        });
    }
}