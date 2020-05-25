const Calc = {
    percentageTo2Decimals: (value, total) => Math.floor(value / total * 10000) / 100
};

export default Calc;