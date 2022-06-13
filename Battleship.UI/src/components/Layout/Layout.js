import React from 'react';
import About from './About/About';
import { Container, Row, Col } from 'react-bootstrap';

const layout = (props) => {
    return (
        <Container>
            <About />
            <Row>
                <Col>{props.children}</Col>
            </Row>
        </Container>
    )
}
export default layout;