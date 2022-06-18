import React from 'react';
import { useState } from 'react';
import { Table, Badge, Button, Spinner } from 'react-bootstrap';
import env from './Environment';

class BattleGrid extends React.Component {
    constructor(props) {
        super(props);
    }

    clickCell(cell) {
        if (this.props.player)
            return;
        this.props.attack(cell.coordinate);
    }
      
    renderCell(cell) {
        return (
            <BattleCell
                player={this.props.player}
                cell={cell}
                onClick={() => this.clickCell(cell)}
            />
        );
    }

    render() {
        const { grid, player } = this.props;
        const offset = player ? 6 : 1;

        const className = 'battle-grid ' + (player ? 'player' : 'computer');

        const slots = [...grid];

        let rows = [];
        while (slots.length >= 10) {
            rows.push(slots.splice(0, 10));
        }
        
        return (
            <>
                <Table className={className} borderless>
                    <tbody>
                    {rows.map((row, i) => (
                        <tr key={i}>
                            <td key={0} className='legend-cell side'><Badge bg='dark'>{i + offset}</Badge></td>
                            {row.map((cell, j) => (<td className='battle-cell' key={j + 1}> {this.renderCell(cell)} </td>))}
                        </tr>
                    ))}
                    </tbody>
                </Table>
            </>
        );
    }
}

const BattleCell = (props) => {
    const { cell, player } = props;
    const disabled = player || cell.isRevealed;
    const theme = player ? env.Theme1 : env.Theme2;

    const handleClick = () => {
        cell.busy = true;
        props.onClick();
    };

    let variant = `outline-${theme}`;

    let content = <span>&nbsp;&nbsp;</span>;
    switch (cell.state) {
        case 'Destroyed':
            content = 'x';
            variant = env.Destroyed;
            break;
        case 'Damaged':
            content = 'x';
            variant = env.Damaged;
            break;
        case 'Occupied':
            variant = theme;
            break;
        case 'Empty':
            if (cell.isRevealed) {
                content = 'x';
            }
            break;
        default:
            break;
    }

    return (
        <>
            <Button variant={variant} size='lg' disabled={disabled} onClick={handleClick}>
                {!cell.busy && content}
                {cell.busy && <Spinner animation="border" size="sm" />}
            </Button>{' '}
        </>
    );
}

export default BattleGrid;