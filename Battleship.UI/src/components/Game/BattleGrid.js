import React from 'react';
import { Table, Badge } from 'react-bootstrap';
import BattleCell from './BattleCell';

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
        const theme = player ? 'info' : 'danger';
        const label = player ? 'Player' : 'Computer';

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
                            {i == 0 && <td className='label-cell' key={6} rowSpan={5}><h3><Badge bg={theme}>{label}</Badge></h3></td>}
                        </tr>
                    ))}
                    </tbody>
                </Table>
            </>
        );
    }
}

export default BattleGrid;