# -*- Mode: Python3; coding: utf-8; indent-tabs-mpythoode: nil; tab-width: 4 -*-
'''
    Expression calculator

    input: infix expression
    output: number
'''

import sys


class Util():

    ERROR = "ERROR"
    DELIM = ' '
    SPACE = ' '

    DIGITS = "0123456789"
    SEPARATOR = ','

    OPERATOR = "+-*/"
    LPARENTHESES = '('
    RPARENTHESES = ')'

    OPERAND = DIGITS + SEPARATOR
    POSTFIX = OPERAND + OPERATOR + SPACE
    ALL = OPERAND + OPERATOR + LPARENTHESES + RPARENTHESES + SPACE

    def isNullOrEmpty(expression):

        if expression is None:
            return True

        return str(expression).replace(Util.SPACE, "") == ""

    def isNumber(value):

        if isinstance(value, int):
            return True

        if isinstance(value, float):
            return True

        if Util.isNullOrEmpty(value):
            return False

        first = value[0]
        last = value[-1]

        if first == '-' or first == '+':
            return Util.isNumber(value[1:])

        if first not in Util.DIGITS:
            return False

        if last not in Util.DIGITS:
            return False

        counter = 0  # parênteses
        for c in value:
            if c not in Util.OPERAND or counter > 1:
                return False
            if c == Util.SEPARATOR:
                counter += 1

        return True

    def validate(expression="", characters=ALL):
        """
        Returns True if all characters in the expression are valid
        and the left and right parentheses count are the same.

        :param expression: string
        :param characters: string (list of valid characters)
        :return bool
        """

        if Util.isNullOrEmpty(expression):
            return False

        counter = 0  # parênteses
        for c in expression:
            if c not in characters:
                return False  # inválido
            if c == Util.LPARENTHESES:
                counter += 1
            if c == Util.RPARENTHESES:
                counter -= 1

        return counter == 0


class ExpressionCalculator():
    """
    Calculate arithmetic expression.

    Main function: calc(expression)
    """

    # Mensagem
    ERROR = Util.ERROR

    def calc(self, expression):
        """
        Returns the calculated value of the infix expression.

        :param expression: string
        :return float
        """

        # InFixo para PosFixo
        postfix = self.postFix(expression)

        # Valor em string, número ou ERROR
        result = self.resolve(postfix)

        if result == self.ERROR:
            return None

        return float(result)

    def resolve(self, postfix, delim=Util.DELIM):
        """
        Calculate expression in PostFix format.

        :param postfix: string (expression delimited by spaces)
        :param delim: string (value delimiter in expression)
        :return string (number or ERROR)
        """

        # Validar entrada Posfixa
        if Util.isNullOrEmpty(postfix):
            return self.ERROR

        # Validar caracteres: considerar entrada de espaços
        if not Util.validate(expression=postfix, characters=Util.POSTFIX):
            return self.ERROR

        # Preparar
        token = []
        for item in postfix.split(delim):
            # Excluir vazios
            if item.replace(Util.SPACE, "") != "":
                token += [item]

        if len(token) == 0:
            return self.ERROR

        if len(token) == 1:
            if Util.isNumber(token[0]):
                return token[0]
            else:
                return self.ERROR

        # Resolver
        numbers = []
        for item in token:
            if item in Util.OPERATOR:
                # Operandos
                operand2 = numbers[-1]  # número
                numbers = numbers[:-1]  # pop
                operand1 = numbers[-1]  # número
                numbers = numbers[:-1]  # pop
                # Calcular
                if item == '+':
                    numbers += [operand1 + operand2]
                if item == '-':
                    numbers += [operand1 - operand2]
                if item == '*':
                    numbers += [operand1 * operand2]
                if item == '/':
                    if operand2 == 0:
                        return self.ERROR
                    numbers += [operand1 / operand2]
            else:
                pySeparator = "."
                numbers += [float(item.replace(Util.SEPARATOR, pySeparator))]

        return str(numbers[-1])

    def postFix(self, infix, delim=Util.DELIM):
        """
        Convert InFix expression to PostFix.

        :param infix: string (expression)
        :param delim: string (value delimiter in expression)
        :return string (postfix expression delimited by spaces)
        """

        # Validar entrada InFixa
        if Util.isNullOrEmpty(infix):
            return self.ERROR

        # Preparação inicial
        infix = str(infix).replace(Util.SPACE, "")

        # Validar caracteres
        if not Util.validate(expression=infix, characters=Util.ALL):
            return self.ERROR

        # Adequar entrada
        infix = self.prepare(infix)

        # Níveis de precedência aritmética
        PRECEDENCE = {
                "*": 3, "/": 3,
                "+": 2, "-": 2,
                "(": 1
        }

        # Converter
        stack = ""
        postfix = Util.SPACE

        for i in range(0, len(infix)):
            # Caracter
            c = infix[i]

            # Vazio
            if c == Util.SPACE:
                continue

            # Operando
            if c in Util.OPERAND:
                # add, dígito ou separador decimal
                postfix += c
                continue

            # Delimitar
            postfix += Util.SPACE if postfix[-1] != Util.SPACE else ""

            # Parênteses
            if c == Util.LPARENTHESES:
                # push, '('
                stack += c
            if c == Util.RPARENTHESES:
                while stack[-1] != Util.LPARENTHESES:
                    # Delimitar
                    postfix += Util.SPACE if postfix[-1] != Util.SPACE else ""
                    # add, topo
                    postfix += stack[-1]
                    # pop, operador
                    stack = stack[:-1]
                # pop, '('
                stack = stack[:-1]

            # Operador
            if c in Util.OPERATOR:
                while (len(stack) > 0 and stack[-1] != Util.LPARENTHESES
                       and PRECEDENCE[c] <= PRECEDENCE[stack[-1]]):
                    # Delimitar
                    postfix += Util.SPACE if postfix[-1] != Util.SPACE else ""
                    # add, topo
                    postfix += stack[-1]
                    # Delimitar
                    postfix += Util.SPACE if postfix[-1] != Util.SPACE else ""
                    # pop
                    stack = stack[:-1]
                # push, operador
                stack += c

        while len(stack) > 0:
            # Delimitar
            postfix += Util.SPACE if postfix[-1] != Util.SPACE else ""
            # add, topo
            postfix += stack[-1]
            # pop
            stack = stack[:-1]

        # retornar expressão, removendo primeiro espaço
        return postfix[1:]

    def prepare(self, expression):
        """
        Format operators joined to negative numbers.

        :param expression: string
        :return string (expression with checked operators)
        """

        # Tratar sinal no início
        if expression[0] is '-' or expression[0] is '+':
            expression = "0" + expression

        # Tratar números negativos
        expression = expression.replace("--", "+")
        expression = expression.replace("+-", "-")
        expression = expression.replace("*+", "*")
        expression = expression.replace("/+", "/")
        expression = expression.replace("*-", "*(0-1)*")
        expression = expression.replace("/-", "*(0-1)/")
        expression = expression.replace("(-", "(0-")

        return expression


if __name__ == '__main__':

    Calculator = ExpressionCalculator()
    expression = "(10+20)/5"

    # Parâmetros de entrada em linha de comando
    for param in sys.argv:
        if param[:5] == "Calc=":
            expression = param.replace("Calc=", "")

    print(ExpressionCalculator.__doc__)
    print("\tEx:")
    print("\tcalc(\"" + expression + "\") = ", Calculator.calc(expression))
