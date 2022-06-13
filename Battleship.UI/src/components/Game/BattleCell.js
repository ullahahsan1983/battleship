import React from "react";
import Button from 'react-bootstrap/Button';
import shipWreck from "./rsz_explosion-png.png";

const BattleCell = (props) => {
    const { cell, player } = props;
    const disabled = player || cell.isRevealed;
    
    let variant = player ? 'outline-info' : 'outline-danger';
    
    let content = <span>&nbsp;</span>;
    switch (cell.state) {
        case 'Damaged':
            content = (<img src={shipWreck} alt="ship-wreck" />);
            variant = player ? 'info' : 'danger';
            break;
        case 'Occupied':
            variant = player ? 'info' : 'danger';
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
            <Button variant={variant} size='lg' disabled={disabled} onClick={props.onClick}>
                {content}
            </Button>{' '}
        </>
    );
}

export default BattleCell;