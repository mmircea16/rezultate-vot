/** @jsx jsx */

import { jsx } from "@emotion/core";
import React, { Component } from "react";
import Button from "@atlaskit/button";
import Select, { components } from "react-select";
import { defaultTheme } from "react-select";

const { colors } = defaultTheme;

const selectStyles = {
  container: provided => ({
    ...provided,
    height: 50
  }),
  control: (provided, state) => ({
    ...provided,
    minWidth: 240,
    backgroundColor: "#E4E4E4",
    borderRadius: 0,
    border: "1px solid #DEDEDE",

    boxShadow: state.isFocused ? "none" : "none",
    "&:hover": {
      border: state.isFocused ? 0 : 0,
      boxShadow: state.isFocused ? "none" : "none"
    },
    margin: "8px 20px"
  }),
  menu: () => ({
    boxShadow: "1px 1px 8px  rgba(0, 0, 0, 0.1)",
    width: "286px",
    backgroundColor: "#fff"
  }),
  option: (provided, state) => ({
    ...provided,
    color: state.isSelected ? "orange" : "grey",
    color: state.isFocused ? "orange" : "grey",
    backgroundColor: state.isSelected ? "#fff" : "#fff",
    fontSize: 16,
    fontWeight: "normal",
    paddingLeft: 20
  }),
  menuList: provided => ({
    ...provided,
    padding: "8px 16px"
  })
};

export default class CountiesSelect extends Component {
  state = { isOpen: false, value: undefined };
  toggleOpen = () => {
    this.setState(state => ({ isOpen: !state.isOpen }));
  };
  onSelectChange = value => {
    this.toggleOpen();
    this.setState({ value });
    this.props.onSelect(value);
  };
  getHeader = value => {
    if (!value) return "Total";
    if (value.id === "diaspora" || value.id === "national" || value.id === "" || value.id === "mail")
      return value.label;
    return `Județ: ${value.label}`;
  };
  render() {
    const { isOpen, value } = this.state;
    return (
      <Dropdown
        isOpen={isOpen}
        onClose={this.toggleOpen}
        target={
          <Button
            className={"counties-select dropdown-button"}
            iconAfter={<ChevronDown />}
            onClick={this.toggleOpen}
            isSelected={isOpen}
          >
            {this.getHeader(value)}
          </Button>
        }
      >
        <Select
          autoFocus
          backspaceRemovesValue={false}
          components={{ DropdownIndicator, MenuList, Option }}
          controlShouldRenderValue={false}
          hideSelectedOptions={false}
          isClearable={false}
          menuIsOpen
          onChange={this.onSelectChange}
          options={this.props.counties}
          placeholder="Cauta..."
          styles={selectStyles}
          tabSelectsValue={false}
          value={value}
        />
      </Dropdown>
    );
  }
}

// styled components

const Menu = props => {
  const shadow = "hsla(218, 50%, 10%, 0.1)";
  return (
    <div
      css={{
        backgroundColor: "white",
        borderRadius: 4,
        boxShadow: `0 0 0 1px ${shadow}, 0 4px 11px ${shadow}`,
        marginTop: -8,
        position: "absolute",
        zIndex: 2,
        width: "286px"
      }}
      {...props}
    />
  );
};
const Blanket = props => (
  <div
    css={{
      bottom: 0,
      left: 0,
      top: 0,
      right: 0,
      position: "fixed",
      zIndex: 1
    }}
    {...props}
  />
);

const Dropdown = ({ children, isOpen, target, onClose }) => (
  <div css={{ position: "relative" }}>
    {target}
    {isOpen ? <Menu>{children}</Menu> : null}
    {isOpen ? <Blanket onClick={onClose} /> : null}
  </div>
);
const Svg = p => (
  <svg
    width="24"
    height="24"
    viewBox="0 0 24 24"
    focusable="false"
    role="presentation"
    {...p}
  />
);
const Option = props => {
  if (props.data.label === "Total") {
    return (
      <div className={"first-option"}>
        <components.Option {...props}>{props.children}</components.Option>
      </div>
    );
  } else if (props.data.label === "Diaspora") {
    return (
      <div className={"first-option"}>
        <components.Option {...props}>{props.children}</components.Option>
      </div>
    );
  } else if (props.data.label === "National") {
    return (
      <div className={"first-option"}>
        <components.Option {...props}>{props.children}</components.Option>
      </div>
    );
  } else if (props.data.label === "Corespondență") {
    return (
        <div className={"last-option"}>
            <components.Option {...props}>{props.children}</components.Option>
        </div>
    );
  } else {
    return (
      <components.Option className={"menu-option"} {...props}>
        {props.children}
      </components.Option>
    );
  }
};

const MenuList = props => {
  const total = props.children[0];
  const diaspora = props.children[1];
  const national = props.children[2];
  const mail = props.children[3];
    if (props.children.length > 3) {
        props.children.shift();
        props.children.shift();
        props.children.shift();
        props.children.shift();
    }

  return (
    <components.MenuList {...props}>
      {total}
      {diaspora}
      {national}
      {mail}
      <div className={"county-text"}>Județ</div>
      {props.children}
    </components.MenuList>
  );
};
const DropdownIndicator = () => (
  <div css={{ color: colors.neutral20, height: 24, width: 32 }}>
    <Svg>
      <path
        d="M16.436 15.085l3.94 4.01a1 1 0 0 1-1.425 1.402l-3.938-4.006a7.5 7.5 0 1 1 1.423-1.406zM10.5 16a5.5 5.5 0 1 0 0-11 5.5 5.5 0 0 0 0 11z"
        fill="#4F4F4F"
        fillRule="evenodd"
      />
    </Svg>
  </div>
);
const ChevronDown = () => (
  <Svg style={{ marginRight: -6 }}>
    <path
      d="M8.292 10.293a1.009 1.009 0 0 0 0 1.419l2.939 2.965c.218.215.5.322.779.322s.556-.107.769-.322l2.93-2.955a1.01 1.01 0 0 0 0-1.419.987.987 0 0 0-1.406 0l-2.298 2.317-2.307-2.327a.99.99 0 0 0-1.406 0z"
      fill="currentColor"
      fillRule="evenodd"
    />
  </Svg>
);
