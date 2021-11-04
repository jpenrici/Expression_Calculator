package calculator;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;

class CalculatorTest {

    private final Calculator t = new Calculator();

    protected void Error() {
        assertEquals(t.ERROR, "ERROR");
    }

    protected void isNullOrEmpty() {
        // Nulo ou vazio
        assertTrue(t.isNullOrEmpty(""));
        assertTrue(t.isNullOrEmpty(" "));
        assertTrue(t.isNullOrEmpty(null));
        // Preenchido com string
        assertFalse(t.isNullOrEmpty("a"));
        assertFalse(t.isNullOrEmpty("-"));
        assertFalse(t.isNullOrEmpty("0"));
        assertFalse(t.isNullOrEmpty("-1"));
        assertFalse(t.isNullOrEmpty("1+1"));
        assertFalse(t.isNullOrEmpty("(10+10.1)"));
        // Preenchido por conversão
        assertFalse(t.isNullOrEmpty(0));           // valor inteiro
        assertFalse(t.isNullOrEmpty(-1));          // valor negativo
        assertFalse(t.isNullOrEmpty(10.1));        // valor fracionado
        assertFalse(t.isNullOrEmpty((float) 10.1));         // valor fracionado
        assertFalse(t.isNullOrEmpty((float) -10.1));        // valor fracionado
        assertFalse(t.isNullOrEmpty(10 + 10.1));   // cálculo
    }

    protected void numbers() {
        // Não numéricos
        assertFalse(t.isNumber(""));
        assertFalse(t.isNumber(" "));
        assertFalse(t.isNumber(null));
        assertFalse(t.isNumber("A"));
        assertFalse(t.isNumber("-"));
        assertFalse(t.isNumber("+"));
        assertFalse(t.isNumber(","));
        assertFalse(t.isNumber("-,"));
        assertFalse(t.isNumber("(10+10.1)"));     // expressão
        // Numéricos com erros
        assertFalse(t.isNumber("0.1"));           // separador decimal != ','
        assertFalse(t.isNumber(".1"));
        assertFalse(t.isNumber("1."));
        assertFalse(t.isNumber(",1"));            // erro digitação
        assertFalse(t.isNumber("1,"));
        assertFalse(t.isNumber("-1,"));
        assertFalse(t.isNumber("-,1"));
        assertFalse(t.isNumber("0,,1"));
        assertFalse(t.isNumber("0,0,1"));
        assertFalse(t.isNumber(",,1"));
        assertFalse(t.isNumber("1 "));
        assertFalse(t.isNumber(" 1"));
        assertFalse(t.isNumber("1A"));
        assertFalse(t.isNumber("0,A"));
        // String numéricas
        assertTrue(t.isNumber("0"));
        assertTrue(t.isNumber("1"));
        assertTrue(t.isNumber("-1"));
        assertTrue(t.isNumber("+1"));
        assertTrue(t.isNumber("10"));
        assertTrue(t.isNumber("0,1"));
        // Numéricos
        assertTrue(t.isNumber(10));
        assertTrue(t.isNumber(-10));
        assertTrue(t.isNumber(10.1));
        assertTrue(t.isNumber(-10.1));
        assertTrue(t.isNumber((float) 10.1));
        assertTrue(t.isNumber((float) -10.1));
        assertTrue(t.isNumber(10 + 10.1));
    }

    protected void validate() {
        // Nulo ou vazio
        assertFalse(t.validate(""));
        assertFalse(t.validate(" "));
        assertFalse(t.validate(null));
        // Caracteres inválidos
        assertFalse(t.validate("a"));
        assertFalse(t.validate("(0,1+-,2/5.)"));  // separador decimal != ','
        assertFalse(t.validate("(01,23+-,4*5,6/7-8+9,)+A"));
        // Parênteses sem par
        assertFalse(t.validate("(((1+2))"));      // '(' > ')'
        assertFalse(t.validate("(((01,23+-,4*5,6/7-8+9,))))"));
        // Somente caracteres válidos
        assertTrue(t.validate("0"));
        assertTrue(t.validate("-"));
        assertTrue(t.validate("-1"));
        assertTrue(t.validate("0+1"));
        assertTrue(t.validate("+(-1)"));
        assertTrue(t.validate("+ ( - 1 )"));
    }

    protected void prepare() {
        // Soma de número negativo '+-'
        assertEquals("1-1", t.prepare("1+-1"));
        // Operador '+' junto ao '-'
        assertEquals("0-1", t.prepare("+-1"));
        // Operador '+' no início
        assertEquals("0+1-2", t.prepare("+1+-2"));
        // Operador '-' no início
        assertEquals("0-1-1", t.prepare("-1-1"));
        // Multiplicação de número negativo '*-'
        assertEquals("1*(0-1)*1", t.prepare("1*-1"));
        // Multiplicação de número positivo '*+'
        assertEquals("1*1", t.prepare("1*+1"));
        // Divisão de número negativo '*-'
        assertEquals("1*(0-1)/1", t.prepare("1/-1"));
        // Divisão de número negativo '*+'
        assertEquals("1/1", t.prepare("1/+1"));
        // Operador '-' próximo a parênteses
        assertEquals("0-(0-(0-15*(0-1)*10)))", t.prepare("-(-(-15*-10)))"));
    }

    protected void postFix() {
        //Inválido
        assertEquals("ERROR", t.postFix(""));
        assertEquals("ERROR", t.postFix(" "));
        assertEquals("ERROR", t.postFix(null));
        assertEquals("ERROR", t.postFix("expression"));
        //Número
        assertEquals("10", t.postFix(10));                //valor inteiro
        assertEquals("10,5", t.postFix((float) 10.5));         //valor fracionado
        assertEquals("10,5", t.postFix(10.5));            //valor fracionado
        assertEquals("0", t.postFix("0"));
        assertEquals("0 1 -", t.postFix(-1));             //valor negativo
        assertEquals("0 1 -", t.postFix("-1"));
        assertEquals("0 0,1 -", t.postFix("-0,1"));       //valor fracionado
        //Expressão
        assertEquals("0 1 - 1 +", t.postFix("- 1 + 1"));  //espaço
        assertEquals("0 1 - 1 +", t.postFix("-1+1"));
        assertEquals("0 1 + 2 -", t.postFix("0+1-2"));
        assertEquals("1 2 + 3 +", t.postFix("1+2+3"));
        assertEquals("1,1 1,1 +", t.postFix("1,1+1,1"));
        assertEquals("1 1 1 + +", t.postFix("1+(1+1)"));
        assertEquals("1 0,1 + 2 +", t.postFix("(1+0,1)+2"));
        assertEquals("1 0,1 * 15 -", t.postFix("(1*0,1)-15"));
        assertEquals("1 0,1 / 1 -", t.postFix("(1/0,1)+-1"));
        assertEquals("1 0,1 / 0 1 - * 1 *", t.postFix("(1/0,1)*-1"));
        assertEquals("1 0,1 / 0 1 - * 1 * 15 +", t.postFix("(1/0,1)*-1+15"));
        assertEquals("1 0,1 / 0 1 - * 5 /", t.postFix("(1/0,1)/-5"));
        assertEquals("1 0,1 / 0 1 - 1 + /", t.postFix("(1/0,1)/(-1+1)"));
        assertEquals("1 1,1 - 0 5 - 1 + -", t.postFix("((1+-1,1))+-((-5+1))"));
        assertEquals("0 0 1 - - 0 2 - 3 + -", t.postFix("-(((-1)))+-(((-2+3)))"));
        assertEquals("0 0 1 - + 0 2 - 3 + -", t.postFix("+(((-1)))+-(((-2+3)))"));
        assertEquals("1,0 4,0 +", t.postFix("1,0+4,0"));
        assertEquals("1,0 4,0 + 2,0 + 3 +", t.postFix("1,0+4,0+2,0+3"));
        assertEquals("5,0 1,0 -", t.postFix("5,0-1,0"));
        assertEquals("5,0 2,0 - 2 -", t.postFix("5,0-2,0-2"));
        assertEquals("5,0 2,0 *", t.postFix("5,0*2,0"));
        assertEquals("5,0 2,0 * 2 *", t.postFix("5,0*2,0*2"));
        assertEquals("10,0 2,0 /", t.postFix("10,0/2,0"));
        assertEquals("10,0 2,0 / 2 / 10 /", t.postFix("10,0/2,0/2/10"));
        assertEquals("10 10 + 5 2 * -", t.postFix("(10+10)-(5*2)"));
        assertEquals("1,5 2,5 + 3 + 2,5 2,5 + -", t.postFix("(1,5+2,5+3)-(2,5+2,5)"));
        assertEquals("5 0 1 - * 10 * 20 + 5 -", t.postFix("5*-10+20-5"));
        assertEquals("2000 1 2 / +", t.postFix("2000+1/2"));
        assertEquals("2 2 + 4 5 * + 1 1000 / +", t.postFix("2+2+4*5+1/1000"));
    }

    protected void resolve() {
        // Inválido
        assertEquals("ERROR", t.resolve(""));
        assertEquals("ERROR", t.resolve(" "));
        assertEquals("ERROR", t.resolve(null));
        assertEquals("ERROR", t.resolve("A"));
        assertEquals("ERROR", t.resolve(",1"));
        assertEquals("ERROR", t.resolve("(1 + 2)"));  // input: infix, esperado: posfix
        // Número
        assertEquals("0", t.resolve("0"));
        assertEquals("0", t.resolve(" 0"));
        assertEquals("1,1", t.resolve("1,1"));
        // Expressão
        assertEquals("0,25", t.resolve("10,0 2,0 / 2 / 10 /"));
        assertEquals("10", t.resolve("10 10 + 5 2 * -"));
        assertEquals("2", t.resolve("1,5 2,5 + 3 + 2,5 2,5 + -"));
        assertEquals("-1", t.resolve("0 1 -"));
        assertEquals("0", t.resolve("0 1 - 1 +"));
        assertEquals("-1", t.resolve("0 1 + 2 -"));
        assertEquals("6", t.resolve("1 2 + 3 +"));
        assertEquals("2,2", t.resolve("1,1 1,1 +"));
        assertEquals("3,1", t.resolve("1 0,1 + 2 +"));
        assertEquals("-14,9", t.resolve("1 0,1 * 15 -"));
        assertEquals("9", t.resolve("1 0,1 / 1 -"));
        assertEquals("-10", t.resolve("1 0,1 / 0 1 - * 1 *"));
        assertEquals("5", t.resolve("1 0,1 / 0 1 - * 1 * 15 +"));
        assertEquals("-2", t.resolve("1 0,1 / 0 1 - * 5 /"));
        assertEquals("ERROR", t.resolve("1 0,1 / 0 1 - 1 + /"));  // Divisão por zero
        assertEquals("3,9", t.resolve("1 1,1 - 0 5 - 1 + -"));
        assertEquals("0", t.resolve("0 0 1 - - 0 2 - 3 + -"));
        assertEquals("-2", t.resolve("0 0 1 - + 0 2 - 3 + -"));
        assertEquals("-35", t.resolve("5 0 1 - * 10 * 20 + 5 -"));
        assertEquals("2000,5", t.resolve("2000 1 2 / +"));
        assertEquals("24,001", t.resolve("2 2 + 4 5 * + 1 1000 / +"));
        assertEquals("2", t.resolve("1,5 2,5 + 3 + 2,5 2,5 + -"));
        assertEquals("757", t.resolve("100 200 + 2 / 5 * 7 +"));
        assertEquals("-4", t.resolve("2 3 1 * + 9 -"));
        assertEquals("23", t.resolve("10 2 8 * + 3 -"));
    }

    protected void calc() {
        assertEquals("0,0", t.expressionCalc("0"));
        assertEquals("1,0", t.expressionCalc("1"));
        assertEquals("-1,0", t.expressionCalc("-1"));
        assertEquals("0,0", t.expressionCalc("-1+1"));
        assertEquals("-1,0", t.expressionCalc("0+1-2"));
        assertEquals("6,0", t.expressionCalc("1+2+3"));
        assertEquals("2,2", t.expressionCalc("1,1+1,1"));
        assertEquals("3,1", t.expressionCalc("(1+0,1)+2"));
        assertEquals("-14,9", t.expressionCalc("(1*0,1)-15"));
        assertEquals("9,0", t.expressionCalc("(1/0,1)+-1"));
        assertEquals("-10,0", t.expressionCalc("(1/0,1)*-1"));
        assertEquals("-10,0", t.expressionCalc("(1/0,1)/-1"));
        assertEquals("ERROR", t.expressionCalc("(1/0,1)/(-1+1)"));
        assertEquals("3,9", t.expressionCalc("((1+-1,1))+-((-5+1))"));
        assertEquals("0,0", t.expressionCalc("-(((-1)))+-(((-2+3)))"));
        assertEquals("-35,0", t.expressionCalc("5*-10+20-5"));
        assertEquals("2000,5", t.expressionCalc("2000+1/2"));
        assertEquals("24,001", t.expressionCalc("2+2+4*5+1/1000"));
        assertEquals("23,0", t.expressionCalc("((10 + (2 * 8)) - 3)"));
    }

    protected void extra() {
        // Outro delimitador
        String value = t.postFix("1+2*5,0+-1", '|');
        assertEquals("1|2|5,0|*|+|1|-", value);
        String result = t.resolve(value, '|');
        assertEquals("10", result);
    }

}