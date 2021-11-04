package calculator;

import java.util.ArrayList;
import java.util.List;
import java.util.Stack;

public class Calculator {

    protected final String ERROR = "ERROR";
    static final String WSPACE = " ";
    static final String EMPTY = "";
    static final char DELIM = ' ';
    static final char SPACE = ' ';
    static final char DOT = '.';

    static final String DIGITS = "0123456789";
    static final char SEPARATOR = ',';

    static final String OPERATOR = "+-*/";
    static final char LPARENTHESES = '(';
    static final char RPARENTHESES = ')';

    static final String OPERAND = DIGITS + SEPARATOR;
    static final String POSTFIX = OPERAND + OPERATOR;
    static final String ALL = POSTFIX + LPARENTHESES + RPARENTHESES + SPACE;

    static int precedence(char key) {
        switch (key) {
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

    String[] split(String str, char delimiter) {
        str = str.replace(delimiter, ';');
        return str.split(";");
    }

    public String expressionCalc(String expression) {

        // InFixo para PosFixo, delimitador padrão (espaço)
        String postfix = postFix(expression, SPACE);

        // Valor em string, número ou ERROR
        String result = resolve(postfix, SPACE);

        if (result.equals("ERROR")) {
            return ERROR;
        }

        String[] array = split(result, SEPARATOR);
        if (array.length == 1) {
            result += ",0";
        }

        return result;
    }

    protected String resolve(String postfix, char delimiter) {

        // Validar entrada Posfixa
        if (isNullOrEmpty(postfix)) {
            return ERROR;
        }

        // Validar caracteres
        if (!validate(postfix, POSTFIX + delimiter)) {
            return ERROR;
        }

        // Preparar
        List<String> tokens = new ArrayList<>();
        for (var token : split(postfix, delimiter)) {
            // Excluir vazios
            if (!token.replace(WSPACE, EMPTY).isEmpty())
                tokens.add(token);
        }

        if (tokens.size() == 0) {
            return ERROR;
        }

        if (tokens.size() == 1) {
            if (isNumber(tokens.get(0)))
                return tokens.get(0);
            else
                return ERROR;
        }

        // Resolver
        Stack<Double> numbers = new Stack<>();
        for (String token : tokens) {
            if (OPERATOR.contains(token)) {
                // Operandos
                var operand2 = numbers.pop();
                var operand1 = numbers.pop();
                // Calcular
                switch (token) {
                    case "+":
                        numbers.push(operand1 + operand2);
                        break;
                    case "-":
                        numbers.push(operand1 - operand2);
                        break;
                    case "*":
                        numbers.push(operand1 * operand2);
                        break;
                    case "/":
                        if (operand2 != 0)
                            numbers.push(operand1 / operand2);
                        else
                            return ERROR;
                        break;
                }
            } else {
                double num = Double.parseDouble(token.replace(SEPARATOR, DOT));
                numbers.push(num);
            }
        }

        if (numbers.size() != 1)
            return ERROR;

        String result = numbers.pop().toString().replace(DOT, SEPARATOR);
        String[] s = split(result, SEPARATOR);
        if (s.length == 2) {
            if (Integer.parseInt(s[s.length - 1]) == 0) {
                result = s[0];
            }
        }

        return result.replace(DOT, SEPARATOR);
    }

    protected String resolve(String infix) {
        return resolve(infix, DELIM);
    }

    protected String postFix(String infix, char delimiter) {

        if (isNullOrEmpty(infix)) {
            return ERROR;
        }

        // Preparação inicial
        infix = infix.replace(WSPACE, EMPTY);

        // Validar caracteres
        if (!validate(infix)) {
            return ERROR;
        }

        // Adequar entrada
        infix = prepare(infix);

        // Converter
        Stack<Character> stack = new Stack<>();
        StringBuilder postfix = new StringBuilder(String.valueOf(delimiter));

        for (int i = 0; i < infix.length(); i++) {
            char c = infix.charAt(i);

            // Vazio
            if (c == SPACE) {
                continue;
            }

            // Operando
            if (OPERAND.contains(String.valueOf(c))) {
                postfix.append(c);
                continue;
            }

            if (postfix.charAt(postfix.length() - 1) != delimiter) {
                postfix.append(delimiter);
            }

            // Parênteses
            if (c == LPARENTHESES) {
                stack.push(c);
            }
            if (c == RPARENTHESES) {
                while (stack.peek() != LPARENTHESES) {
                    if (postfix.charAt(postfix.length() - 1) != delimiter)
                        postfix.append(delimiter);
                    postfix.append(stack.pop().toString());
                }
                stack.pop();
            }

            // Operador
            if (OPERATOR.contains(String.valueOf(c))) {
                while (stack.size() > 0 && stack.peek() != LPARENTHESES &&
                        precedence(c) <= precedence(stack.peek())) {
                    if (postfix.charAt(postfix.length() - 1) != delimiter)
                        postfix.append(delimiter);
                    postfix.append(stack.pop().toString()).append(delimiter);
                }
                stack.push(c);
            }
        }

        while (stack.size() > 0) {
            if (postfix.charAt(postfix.length() - 1) != delimiter)
                postfix.append(delimiter);
            postfix.append(stack.pop().toString());
        }

        if (postfix.length() > 1) {
            if (postfix.charAt(0) == delimiter) {
                postfix = new StringBuilder(postfix.substring(1));
            }
        }

        return postfix.toString();
    }

    protected String postFix(String infix) {
        return postFix(infix, DELIM);
    }

    protected String postFix(int infix) {
        return postFix(String.valueOf(infix));
    }

    protected String postFix(double infix) {
        return postFix(String.valueOf(infix).replace(DOT, SEPARATOR));
    }

    protected String prepare(String expression) {

        // Tratar sinal no início
        if (expression.charAt(0) == '-' || expression.charAt(0) == '+')
            expression = "0" + expression;

        // Tratar números com sinais
        expression = expression.replace("--", "+");
        expression = expression.replace("+-", "-");
        expression = expression.replace("*+", "*");
        expression = expression.replace("/+", "/");
        expression = expression.replace("*-", "*(0-1)*");
        expression = expression.replace("/-", "*(0-1)/");
        expression = expression.replace("(-", "(0-");

        return expression;
    }

    protected boolean validate(String expression, String characters) {

        if (isNullOrEmpty(expression))
            return false;

        int lpar = 0;
        int rpar = 0;
        for (int i = 0; i < expression.length(); i++) {
            char c = expression.charAt(i);
            if (!characters.contains(String.valueOf(c))) {
                return false;
            }
            if (c == LPARENTHESES) {
                lpar++;
            }
            if (c == RPARENTHESES) {
                rpar++;
            }
        }

        return lpar == rpar;
    }

    protected boolean validate(String expression) {
        return validate(expression, ALL);
    }

    protected boolean isNumber(String value) {

        if (isNullOrEmpty(value)) {
            return false;
        }

        char first = value.charAt(0);
        char last = value.charAt(value.length() - 1);

        if (first == '-' || first == '+') {
            return isNumber(value.substring(1));
        }

        if (!DIGITS.contains(String.valueOf(first)) || !DIGITS.contains(String.valueOf(last))) {
            return false;
        }

        int counter = 0;  // parênteses
        for (int i = 0; i < value.length(); i++) {
            char c = value.charAt(i);
            if (!OPERAND.contains(String.valueOf(c)) || counter > 1)
                return false;
            if (c == SEPARATOR)
                counter += 1;
        }

        return true;
    }

    protected boolean isNumber(int value) {
        return isNumber(String.valueOf(value));
    }

    protected boolean isNumber(float value) {
        return isNumber((double) value);
    }

    protected boolean isNumber(double value) {
        String num = String.valueOf(value).replace(DOT, SEPARATOR);
        return isNumber(num);
    }

    protected boolean isNullOrEmpty(String expression) {

        if (expression == null) {
            return true;
        }

        return expression.isEmpty() || expression.isBlank();
    }

    protected boolean isNullOrEmpty(int expression) {
        return isNullOrEmpty(String.valueOf(expression));
    }

    protected boolean isNullOrEmpty(float expression) {
        return isNullOrEmpty(String.valueOf(expression));
    }

    protected boolean isNullOrEmpty(double expression) {
        return isNullOrEmpty(String.valueOf(expression));
    }

}
