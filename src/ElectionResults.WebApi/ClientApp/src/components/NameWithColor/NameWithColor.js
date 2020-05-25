import React from "react";
import "./NameWithColor.css"

const NameWithColor = ({text, color}) => <div className={"name-with-color"}><div className={"color-marker"} style={{backgroundColor: color}}/>{text}</div>;

export default NameWithColor;
