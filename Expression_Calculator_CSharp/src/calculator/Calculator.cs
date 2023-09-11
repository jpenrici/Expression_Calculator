using System.Collections.Generic;
using System.Linq;
using System;

namespace MathCalc
{
    public class Calculator
    {
        #region Fields
        public readonly string ERROR = "ERROR";
        public const string WSPACE   = " ";
        const char SPACE = ' ';
        const char DELIM = ' ';

        const string DIGITS  = "0123456789";
        const char SEPARATOR = ',';

        const string OPERATOR = "+-*/";
        public const char LPARENTHESES = '(';
        public const char RPARENTHESES = ')';

        static readonly string OPERAND = DIGITS + SEPARATOR;
        static readonly string POSTFIX = OPERAND + OPERATOR;
        static readonly string ALL = POSTFIX + LPARENTHESES + RPARENTHESES + SPACE;
        #endregion

        static int PRECEDENCE(char key)
        {
            switch (key)
            {
                case '*':
                case '/':
                    return 3;
                case '+':
                case '-':
                    return 2;
                case '(':
                    return 1;
                default:
                    return 0;
            }
        }

        public string ExpressionCalc(string expression)
        {
            // InFixo para PosFixo, delimitador padrão (espaço)
            var postfix = PostFix(expression, SPACE);

            // Valor em string, número ou ERROR
            var result = Resolve(postfix, SPACE);

            if (result.Equals("ERROR"))
                return ERROR;

            if (!result.Any(c => c == SEPARATOR))
                result += ",0";

            return result;
        }

        #region Resolve
        public string Resolve(string infix) => Resolve(infix, DELIM);

        public string Resolve(string postfix, char delim)
        {
            // Validar entrada Posfixa
            if (IsNullOrEmpty(postfix))
                return ERROR;

            // Validar caracteres: considerar entrada de espaços
            if (!Validate(postfix, POSTFIX + delim))
                return ERROR;

            // Preparar
            List<string> tokens = new List<string>();
            foreach (var item in postfix.Split(delim))
            {
                // Excluir vazios
                if (item.Replace(WSPACE, String.Empty) != String.Empty)
                    tokens.Add(item);
            }

            if (tokens.Count == 0)
                return ERROR;

            if (tokens.Count == 1)
            {
                if (IsNumber(tokens[0]))
                    return tokens[0];
                else
                    return ERROR;
            }

            // Resolver
            Stack<double> numbers = new Stack<double>();
            foreach (var i in tokens)
            {
                if (OPERATOR.Contains(i))
                {
                    // Operandos
                    var operand2 = numbers.Pop();
                    var operand1 = numbers.Pop();
                    // Calcular
                    switch (i)
                    {
                        case "+":
                            numbers.Push(operand1 + operand2);
                            break;
                        case "-":
                            numbers.Push(operand1 - operand2);
                            break;
                        case "*":
                            numbers.Push(operand1 * operand2);
                            break;
                        case "/":
                            if (!operand2.Equals(0))
                                numbers.Push(operand1 / operand2);
                            else
                                return ERROR;
                            break;
                    }
                }
                else
                {
                    numbers.Push(Double.Parse(i));
                }
            }

            if (numbers.Count != 1)
                return ERROR;

            var result = numbers.Pop().ToString();

            return result;
        }
        #endregion

        #region PostFix
        public string PostFix(string infix, char delim)
        {
            if (IsNullOrEmpty(infix))
                return ERROR;

            // Preparação inicial
            infix = infix.Replace(SPACE.ToString(), String.Empty);

            // Validar caracteres
            if (!Validate(infix))
                return ERROR;

            // Adequar entrada
            infix = Prepare(infix);

            // Converter
            Stack<char> stack = new Stack<char>();
            string postfix = delim.ToString();

            foreach (var c in infix)
            {
                // Vazio
                if (c == SPACE)
                    continue;

                // Operando
                if (OPERAND.Contains(c))
                {
                    postfix += c;
                    continue;
                }

                if (postfix[postfix.Length - 1] != delim)
                    postfix += delim;

                // Parênteses
                if (c == LPARENTHESES)
                    stack.Push(c);
                if (c == RPARENTHESES)
                {
                    while (stack.Peek() != LPARENTHESES)
                    {
                        if (postfix[postfix.Length - 1] != delim)
                            postfix += delim;
                        postfix += stack.Pop().ToString();
                    }
                    stack.Pop();
                }

                // Operador
                if (OPERATOR.Contains(c))
                {
                    while (stack.Count > 0 &&
                        stack.Peek() != LPARENTHESES &&
                        PRECEDENCE(c) <= PRECEDENCE(stack.Peek()))
                    {
                        if (postfix[postfix.Length - 1] != delim)
                            postfix += delim;
                        postfix += stack.Pop().ToString() + delim;
                    }
                    stack.Push(c);
                }
            }

            while (stack.Count > 0)
            {
                if (postfix[postfix.Length - 1] != delim)
                    postfix += delim;
                postfix += stack.Pop().ToString();
            }

            if (postfix.Length > 1)
            {
                if (postfix[0] == delim)
                    postfix = postfix.Remove(0, 1);
            }

            return postfix;
        }

        public string PostFix(string infix) => PostFix(infix, DELIM);

        public string PostFix(int infix) => PostFix(infix.ToString());

        public string PostFix(double infix) => PostFix(infix.ToString());
        #endregion

        public string Prepare(string expression)
        {
            var str = expression;

            // Tratar sinal no início
            if (str[0] == '-' || str[0] == '+')
                str = "0" + str;

            // Tratar números com sinais
            str = str.Replace("--", "+");
            str = str.Replace("+-", "-");
            str = str.Replace("*+", "*");
            str = str.Replace("/+", "/");
            str = str.Replace("*-", "*(0-1)*");
            str = str.Replace("/-", "*(0-1)/");
            str = str.Replace("(-", "(0-");

            return str;
        }

        #region IsNullOrEmpty
        public bool IsNullOrEmpty(int expression) => IsNullOrEmpty(expression.ToString());

        public bool IsNullOrEmpty(float expression) => IsNullOrEmpty(expression.ToString());

        public bool IsNullOrEmpty(double expression) => IsNullOrEmpty(expression.ToString());

        public bool IsNullOrEmpty(string expression)
        {
            if (expression == null)
                return true;

            if (expression is string)
                return expression.Replace(SPACE.ToString(), String.Empty) == String.Empty;

            return false;
        }
        #endregion

        #region IsNumber
        public bool IsNumber(int value) => IsNumber(value.ToString());

        public bool IsNumber(float value) => IsNumber(value.ToString());

        public bool IsNumber(double value) => IsNumber(value.ToString());

        public bool IsNumber(string value)
        {
            if (IsNullOrEmpty(value))
                return false;

            char first = value[0];
            char last = value[value.Length - 1];

            if (first == '-' || first == '+')
                return IsNumber(value.Remove(0, 1));

            if (!DIGITS.Contains(first) || !DIGITS.Contains(last))
                return false;

            int counter = 0;  // parênteses
            foreach (var c in value)
            {
                if (!OPERAND.Contains(c) || counter > 1)
                    return false;
                if (c == SEPARATOR)
                    counter += 1;
            }

            return true;
        }
        #endregion

        #region Validate
        public bool Validate(string expression) => Validate(expression, ALL);

        public bool Validate(string expression, string characters)
        {
            if (IsNullOrEmpty(expression))
                return false;

            foreach (var c in expression)
            {
                if (!characters.Contains(c))
                    return false;
            }

            var lpar = expression.Count(c => c == LPARENTHESES);
            var rpar = expression.Count(c => c == RPARENTHESES);
            if (lpar != rpar)
                return false;

            return true;
        }
        #endregion
    }
}
