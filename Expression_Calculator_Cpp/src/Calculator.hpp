#ifndef CALCULATOR_HPP_
#define CALCULATOR_HPP_

#include <cmath>
#include <iostream>
#include <list>
#include <regex>
#include <stack>
#include <string>
#include <vector>


class Calculator {

    std::string currentCharacters;
    char currentDelimiter;

    std::string ERROR{"ERROR"};
    char SPACE{32};
    char EMPTY{'\0'};

    char SEPARATOR{','};
    char DELIMITER{32};

    std::string DIGITS{"0123456789"};
    std::string OPERATOR{"+-*/"};
    std::string SIGNAL{"+-"};

    char LPARENTHESES = '(';
    char RPARENTHESES = ')';
    char POSITIVE = '+';
    char NEGATIVE = '-';
    char DOT = '.';

    std::string NUMBER  = DIGITS + SEPARATOR;
    std::string POSTFIX = NUMBER + OPERATOR;
    std::string ALL = POSTFIX + LPARENTHESES + RPARENTHESES + DELIMITER;

public:

    Calculator();
    Calculator(const Calculator &) = delete;
    Calculator(Calculator &&) = delete;
    auto operator=(const Calculator &) -> Calculator & = delete;
    auto operator=(Calculator &&) -> Calculator & = delete;

    explicit Calculator(char currentDelimiter);

    virtual ~Calculator() = default;

    auto calculate(const std::string &expression) -> std::string;
    auto expressionIsValid(const std::string &expression, bool isPostFix = false) -> bool;
    auto isEmpty(const std::string &expression) -> bool;
    auto isNumber(const std::string &value) -> bool;
    auto postFix(std::string expression) -> std::string;
    auto precedence(char key) -> int;
    auto prepare(std::string expression) -> std::string;
    auto resolve(const std::string &postfixExpression) -> std::string;
    void setCurrentDelimiter(char currentDelimiter);

    static auto contains(const std::string &str, const char &character) -> bool;
    static auto contains(const std::string &str, const std::string &characters) -> bool;
    static auto replace(const std::string &str, char character, char newCharacter) -> std::string;
    static auto replace(const std::string &str, const std::string &characters, const std::string &strReplace) -> std::string;
    static auto rtrimZeros(const std::string &str) -> std::string;
    static auto split(const std::string &str, char delimiter) -> std::vector<std::string>;

};

#endif
