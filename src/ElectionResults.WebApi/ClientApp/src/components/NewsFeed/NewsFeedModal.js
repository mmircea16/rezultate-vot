import React from "react";
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from "reactstrap";

const NewsFeedModal = props => {
  return (
    <div>
      <Modal isOpen={props.modal} toggle={props.toggle}>
        <ModalHeader toggle={props.toggle}>Imagini</ModalHeader>
        <ModalBody className="img-ribbon">
          <ul>
            {props.imgArray !== null
              ? props.imgArray.map(item => {
                  return (
                    <li>
                      <img key={item.id} src={item.src} />
                    </li>
                  );
                })
              : null}
          </ul>
        </ModalBody>
      </Modal>
    </div>
  );
};

export default NewsFeedModal;
