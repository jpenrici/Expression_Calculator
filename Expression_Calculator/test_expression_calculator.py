# -*- Mode: Python3; coding: utf-8; indent-tabs-mpythoode: nil; tab-width: 4 -*-
'''
    Test Expression calculator
'''

from expression_calculator import ExpressionCalculator, Util


def test_ERROR():
    # Mensagem de erro
    assert (ExpressionCalculator.ERROR == "ERROR")
    print("test_ERROR ... ok")


def test_isNullOrEmpty():
    # Nulo ou vazio
    assert (Util.isNullOrEmpty("") is True)
    assert (Util.isNullOrEmpty(" ") is True)
    assert (Util.isNullOrEmpty(None) is True)
    # Preenchido com string
    assert (Util.isNullOrEmpty("a") is False)
    assert (Util.isNullOrEmpty("-") is False)
    assert (Util.isNullOrEmpty("0") is False)
    assert (Util.isNullOrEmpty('1') is False)
    assert (Util.isNullOrEmpty("-1") is False)
    assert (Util.isNullOrEmpty("1+1") is False)
    assert (Util.isNullOrEmpty("(10+10.1)") is False)
    # Preenchido por conversão
    assert (Util.isNullOrEmpty(0) is False)          # valor inteiro
    assert (Util.isNullOrEmpty(-1) is False)         # valor negativo
    assert (Util.isNullOrEmpty(10.1) is False)       # valor fracionado
    assert (Util.isNullOrEmpty(10+10.1) is False)    # cálculo
    print("test_isNullOrEmpty ... ok")


def test_isNumber():
    # Não numéricos
    assert (Util.isNumber("") is False)
    assert (Util.isNumber(" ") is False)
    assert (Util.isNumber(None) is False)
    assert (Util.isNumber("A") is False)
    assert (Util.isNumber("-") is False)
    assert (Util.isNumber("+") is False)
    assert (Util.isNumber(",") is False)
    assert (Util.isNumber("-,") is False)
    assert (Util.isNumber("(10+10.1)") is False)     # expressão
    # Numéricos com erros
    assert (Util.isNumber("0.1") is False)           # separador decimal != ','
    assert (Util.isNumber(".1") is False)
    assert (Util.isNumber("1.") is False)
    assert (Util.isNumber(",1") is False)            # erro digitação
    assert (Util.isNumber("1,") is False)
    assert (Util.isNumber("-1,") is False)
    assert (Util.isNumber("-,1") is False)
    assert (Util.isNumber("0,,1") is False)
    assert (Util.isNumber("0,0,1") is False)
    assert (Util.isNumber(",,1") is False)
    assert (Util.isNumber("1 ") is False)
    assert (Util.isNumber(" 1") is False)
    assert (Util.isNumber("1A") is False)
    assert (Util.isNumber("0,A") is False)
    # String numéricas
    assert (Util.isNumber("0") is True)
    assert (Util.isNumber("1") is True)
    assert (Util.isNumber("-1") is True)
    assert (Util.isNumber("+1") is True)
    assert (Util.isNumber("10") is True)
    assert (Util.isNumber("0,1") is True)
    # Numéricos
    assert (Util.isNumber(-10) is True)
    assert (Util.isNumber(10.1) is True)
    assert (Util.isNumber(10+10.1) is True)
    print("test_isNumber ... ok")


def test_validate():
    # Nulo ou vazio
    assert (Util.validate("") is False)
    assert (Util.validate(" ") is False)
    assert (Util.validate(None) is False)
    # Caracteres inválidos
    assert (Util.validate("a") is False)
    assert (Util.validate("(0,1+-,2/5.)") is False)  # separador decimal != ','
    assert (Util.validate("(((01,23+-,4*5,6/7-8+9,))))+A") is False)
    # Parênteses sem par
    assert (Util.validate("(((1+2))") is False)
    assert (Util.validate("(((01,23+-,4*5,6/7-8+9,))))") is False)
    # Todos caracteres válidos
    assert (Util.validate("0") is True)
    assert (Util.validate("-") is True)
    assert (Util.validate("-1") is True)
    assert (Util.validate("0+1") is True)
    assert (Util.validate("+(-1)") is True)
    assert (Util.validate("+ ( - 1 )") is True)
    print("test_validate ... ok")


def test_prepare():
    t = ExpressionCalculator()
    # Soma de número negativo '+-'
    assert (t.prepare("1+-1") == "1-1")
    # Operador '+' junto ao '-'
    assert (t.prepare("+-1") == "0-1")
    # Operador '+' no início
    assert (t.prepare("+1+-2") == "0+1-2")
    # Operador '-' no início
    assert (t.prepare("-1-1") == "0-1-1")
    # Multiplicação de número negativo '*-'
    assert (t.prepare("1*-1") == "1*(0-1)*1")
    # Multiplicação de número positivo '*+'
    assert (t.prepare("1*+1") == "1*1")
    # Divisão de número negativo '*-'
    assert (t.prepare("1/-1") == "1*(0-1)/1")
    # Divisão de número negativo '*+'
    assert (t.prepare("1/+1") == "1/1")
    # Operador '-' próximo a parênteses
    assert (t.prepare("-(-(-15*-10)))") == "0-(0-(0-15*(0-1)*10)))")
    print("test_prepare ... ok")


def test_postFix():
    t = ExpressionCalculator()
    # Inválido
    assert (t.postFix("") == "ERROR")
    assert (t.postFix(" ") == "ERROR")
    assert (t.postFix(None) == "ERROR")
    assert (t.postFix("expression") == "ERROR")
    # Número
    assert (t.postFix(10) == "10")                  # valor inteiro
    assert (t.postFix("0") == "0")
    assert (t.postFix(-1) == "0 1 -")               # valor negativo
    assert (t.postFix("-1") == "0 1 -")
    assert (t.postFix("-0,1") == "0 0,1 -")         # valor fracionado
    # Expressão
    assert (t.postFix("- 1 + 1") == "0 1 - 1 +")    # espaço
    assert (t.postFix("-1+1") == "0 1 - 1 +")
    assert (t.postFix("0+1-2") == "0 1 + 2 -")
    assert (t.postFix("1+2+3") == "1 2 + 3 +")
    assert (t.postFix("1,1+1,1") == "1,1 1,1 +")
    assert (t.postFix("1+(1+1)") == "1 1 1 + +")
    assert (t.postFix("(1+0,1)+2") == "1 0,1 + 2 +")
    assert (t.postFix("(1*0,1)-15") == "1 0,1 * 15 -")
    assert (t.postFix("(1/0,1)+-1") == "1 0,1 / 1 -")
    assert (t.postFix("(1/0,1)*-1") == "1 0,1 / 0 1 - * 1 *")
    assert (t.postFix("(1/0,1)*-1+15") == "1 0,1 / 0 1 - * 1 * 15 +")
    assert (t.postFix("(1/0,1)/-5") == "1 0,1 / 0 1 - * 5 /")
    assert (t.postFix("(1/0,1)/(-1+1)") == "1 0,1 / 0 1 - 1 + /")
    assert (t.postFix("((1+-1,1))+-((-5+1))") == "1 1,1 - 0 5 - 1 + -")
    assert (t.postFix("-(((-1)))+-(((-2+3)))") == "0 0 1 - - 0 2 - 3 + -")
    assert (t.postFix("+(((-1)))+-(((-2+3)))") == "0 0 1 - + 0 2 - 3 + -")
    assert (t.postFix("1,0+4,0") == "1,0 4,0 +")
    assert (t.postFix("1,0+4,0+2,0+3") == "1,0 4,0 + 2,0 + 3 +")
    assert (t.postFix("5,0-1,0") == "5,0 1,0 -")
    assert (t.postFix("5,0-2,0-2") == "5,0 2,0 - 2 -")
    assert (t.postFix("5,0*2,0") == "5,0 2,0 *")
    assert (t.postFix("5,0*2,0*2") == "5,0 2,0 * 2 *")
    assert (t.postFix("10,0/2,0") == "10,0 2,0 /")
    assert (t.postFix("10,0/2,0/2/10") == "10,0 2,0 / 2 / 10 /")
    assert (t.postFix("(10+10)-(5*2)") == "10 10 + 5 2 * -")
    assert (t.postFix("(1,5+2,5+3)-(2,5+2,5)") == "1,5 2,5 + 3 + 2,5 2,5 + -")
    assert (t.postFix("5*-10+20-5") == "5 0 1 - * 10 * 20 + 5 -")
    assert (t.postFix("2000+1/2") == "2000 1 2 / +")
    assert (t.postFix("2+2+4*5+1/1000") == "2 2 + 4 5 * + 1 1000 / +")
    print("test_postFix ... ok")


def test_resolve():
    t = ExpressionCalculator()
    # Inválido
    assert (t.resolve("") == "ERROR")
    assert (t.resolve(" ") == "ERROR")
    assert (t.resolve(None) == "ERROR")
    assert (t.resolve("A") == "ERROR")
    assert (t.resolve(",1") == "ERROR")
    assert (t.resolve("(1 + 2)") == "ERROR")  # input: infix, esperado: posfix
    # Número
    assert (t.resolve("0") == "0")
    assert (t.resolve(" 0") == "0")
    assert (t.resolve("1,1") == "1,1")
    # Expressão
    assert (t.resolve("10,0 2,0 / 2 / 10 /") == "0.25")
    assert (t.resolve("10 10 + 5 2 * -") == "10.0")
    assert (t.resolve("1,5 2,5 + 3 + 2,5 2,5 + -") == "2.0")
    assert (t.resolve("0 1 -") == "-1.0")
    assert (t.resolve("0 1 - 1 +") == "0.0")
    assert (t.resolve("0 1 + 2 -") == "-1.0")
    assert (t.resolve("1 2 + 3 +") == "6.0")
    assert (t.resolve("1,1 1,1 +") == "2.2")
    assert (t.resolve("1 0,1 + 2 +") == "3.1")
    assert (t.resolve("1 0,1 * 15 -") == "-14.9")
    assert (t.resolve("1 0,1 / 1 -") == "9.0")
    assert (t.resolve("1 0,1 / 0 1 - * 1 *") == "-10.0")
    assert (t.resolve("1 0,1 / 0 1 - * 1 * 15 +") == "5.0")
    assert (t.resolve("1 0,1 / 0 1 - * 5 /") == "-2.0")
    assert (t.resolve("1 0,1 / 0 1 - 1 + /") == "ERROR")  # Divisão por zero
    assert (t.resolve("1 1,1 - 0 5 - 1 + -") == "3.9")
    assert (t.resolve("0 0 1 - - 0 2 - 3 + -") == "0.0")
    assert (t.resolve("0 0 1 - + 0 2 - 3 + -") == "-2.0")
    assert (t.resolve("5 0 1 - * 10 * 20 + 5 -") == "-35.0")
    assert (t.resolve("2000 1 2 / +") == "2000.5")
    assert (t.resolve("2 2 + 4 5 * + 1 1000 / +") == "24.001")
    assert (t.resolve("1,5 2,5 + 3 + 2,5 2,5 + -") == "2.0")
    print("test_resolve ... ok")


def test_calc():
    t = ExpressionCalculator()
    assert (t.calc("0") == 0)
    assert (t.calc("1") == 1)
    assert (t.calc("-1") == -1)
    assert (t.calc("-1+1") == 0)
    assert (t.calc("0+1-2") == -1)
    assert (t.calc("1+2+3") == 6)
    assert (t.calc("1,1+1,1") == 2.2)
    assert (t.calc("(1+0,1)+2") == 3.1)
    assert (t.calc("(1*0,1)-15") == -14.9)
    assert (t.calc("(1/0,1)+-1") == 9.0)
    assert (t.calc("(1/0,1)*-1") == -10.0)
    assert (t.calc("(1/0,1)/-1") == -10.0)
    assert (t.calc("(1/0,1)/(-1+1)") is None)
    assert (t.calc("((1+-1,1))+-((-5+1))") == 3.9)
    assert (t.calc("-(((-1)))+-(((-2+3)))") == 0)
    assert (t.calc("5*-10+20-5") == -35)
    assert (t.calc("2000+1/2") == 2000.5)
    assert (t.calc("2+2+4*5+1/1000") == 24.001)
    print("test_calc ... ok")


if __name__ == '__main__':

    test_ERROR()
    test_isNullOrEmpty()
    test_isNumber()
    test_validate()
    test_prepare()
    test_postFix()
    test_resolve()
    test_calc()
