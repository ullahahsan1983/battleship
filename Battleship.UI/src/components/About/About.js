import React, { useState } from 'react';
import { Offcanvas, Button, Image } from 'react-bootstrap';
import Avatar from './Avatar.png';
import './Style.css';

const MyProfile = () => {
  return (
    <div className="profile">
      <div className="profile-inner">
        <div className="profile-front">
          <Image src={Avatar} thumbnail />
        </div>
        <div className="profile-back">
          <div className="social-links link-group">
            <a as="button" href="https://www.linkedin.com/in/ahsan-ullah-665aa7215"><span role='img' className="bi-linkedin"></span></a>
          </div>
          <div className="contact-links link-group">
            <span><span className="fa fa-phone"></span>  <span className="contact-info">+8801973906968</span></span>
            <span><span className="fa fa-envelope"></span>  <span className="contact-info">ullahahsan1983@gmail.com</span></span>
          </div>          
          <div className="profile-title">
            Ahsan Ullah
          </div>        
        </div>
      </div>
    </div>
  );
}

const About = (props) => {
  const [show, setShow] = useState(false);

  const handleShow = () => setShow(true);
  const handleClose = () => setShow(false);

  const backdrop = false;
  const className = `about-anchor ${props.variant || 'modern'}`;

  return (
    <>
      <Button onClick={handleShow} variant="info" className={className}>
        About
      </Button>
      <Offcanvas 
        show={show} 
        onHide={handleClose} 
        placement="start"
        backdrop={backdrop}
      >
        <Offcanvas.Header closeButton>
          <Offcanvas.Title>About</Offcanvas.Title>
        </Offcanvas.Header>
        <Offcanvas.Body>
          <div>Platform: <span className="text-warning">React</span></div>
          <p> 
            <div><a href="https://github.com/ullahahsan1983/battleship">Github</a> </div>
          </p>
          <div>Developed by</div>
          <MyProfile />
        </Offcanvas.Body>
      </Offcanvas> 
    </>
  );
}

export default About;

