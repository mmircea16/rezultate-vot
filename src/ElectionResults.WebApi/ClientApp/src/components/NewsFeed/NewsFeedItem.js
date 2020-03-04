import React, { useState } from "react";
import {
  Card,
  CardImg,
  CardText,
  CardBody,
  CardHeader,
  CardTitle,
  CardSubtitle,
  CardFooter,
  Button,
  Media
} from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import twitterLogo from "../../images/twitter_grey.png";
import facebookLogo from "../../images/facebook_grey.png";
import code4Ro from "../../images/code4Romania.svg";
import NewsFeedModal from "./NewsFeedModal";

import "./NewsFeedItem.css";

const NewsFeedItem = props => {
  const [modal, setModal] = useState(false);

  const toggle = () => setModal(!modal);

  let imgArray = [
    { id: 1, src: "rezultate-vot-cover-v2.jpg" },
    { id: 2, src: "rezultate-vot-cover-v2.jpg" },
    { id: 3, src: "rezultate-vot-cover-v2.jpg" }
  ];
  return (
    <div>
      <Card className="newsFeedCard">
        <CardHeader></CardHeader>

        <CardBody className="newsFeedBody">
          <div className="left-half">
            <span className="text-bold">10 Nov 2019</span> 10:22
          </div>
          <div className="right-half">
            <div className="content-header">
              <div>
                <img
                  src={twitterLogo}
                  className="newsFeedProfileImg"
                  width={48}
                  height={48}
                  alt="Sender profile pic"
                />
                <p>Dan Dincu</p>
              </div>

              <p className="text-time">
                <span className="text-bold">10 Nov 2019</span> 10:22
              </p>
            </div>

            <div className="content-body">
              <p className="text-caption">
                Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer
                id maximus nunc. Donec gravida non erat sed dictum.
              </p>
              <p>
                Fusce quis lectus ut turpis ornare feugiat. Maecenas enim mi,
                ornare ut dui sed, fringilla ultrices nisi. Integer sit amet
                molestie justo. Ut faucibus vitae turpis ut sollicitudin. Ut
                metus sapien, ultricies vitae odio nec, pulvinar iaculis mi.
              </p>
              <div className="img-ribbon">
                {imgArray !== null
                  ? imgArray
                      .filter((i, index) => index < 2)
                      .map(item => {
                        return <img key={item.id} src={item.src} />;
                      })
                  : null}
                {imgArray.length > 2 && (
                  <Button className="btn-plus" onClick={toggle}>
                    &#43;
                  </Button>
                )}
              </div>
            </div>
            <div className="content-footer">
              <div>
                <a
                  href="https://www.facebook.com/code4romania"
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  <img
                    src={facebookLogo}
                    className="newsFeedSocialHandles"
                    width={30}
                    height={30}
                    alt="Facebook social handle"
                  />
                </a>
                <a
                  href="https://code4.ro/ro/"
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  <img
                    src={twitterLogo}
                    className="newsFeedSocialHandles"
                    width={30}
                    height={30}
                    alt="Twitter social handle"
                  />
                </a>
              </div>

              <Media height={48} src={code4Ro} />
            </div>
          </div>
        </CardBody>
      </Card>

      <NewsFeedModal
        modal={modal}
        imgArray={imgArray}
        toggle={() => toggle()}
      />
    </div>
  );
};

export default NewsFeedItem;
