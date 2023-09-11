// Compile Linux/Mono: mcs -target:exe -out:Test.exe CalculatorTest.cs Calculator.cs
using System;
using System.Diagnostics;
using MathCalc;

namespace TestMathCalc
{
    public class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine("Start test ...");

            test.Error();
            test.IsNullOrEmpty();
            test.Numbers();
            test.Validate();
            test.Prepare();
            test.PostFix();
            test.Resolve();
            test.Calc();
            test.Extra();

            watch.Stop();
            var milliseconds = watch.ElapsedMilliseconds;

            Console.WriteLine("Finished tests.\nElapsed {0} miliseconds", milliseconds);
        }
    }

    public class Test
    {
        MathCalc.Calculator t = new MathCalc.Calculator();

        public void Error()
        {
            Debug.Assert(t.ERROR == "ERROR");

            Console.WriteLine("Error test finished!");
        }

        public void IsNullOrEmpty()
        {
            // Nulo ou vazio
            Debug.Assert(true == t.IsNullOrEmpty(""));
            Debug.Assert(true == t.IsNullOrEmpty(" "));
            Debug.Assert(true == t.IsNullOrEmpty(null));
            // Preenchido com string
            Debug.Assert(false == t.IsNullOrEmpty("a"));
            Debug.Assert(false == t.IsNullOrEmpty("-"));
            Debug.Assert(false == t.IsNullOrEmpty("0"));
            Debug.Assert(false == t.IsNullOrEmpty("-1"));
            Debug.Assert(false == t.IsNullOrEmpty("1+1"));
            Debug.Assert(false == t.IsNullOrEmpty("(10+10.1)"));
            // Preenchido por conversão
            Debug.Assert(false == t.IsNullOrEmpty(0));           // valor inteiro
            Debug.Assert(false == t.IsNullOrEmpty(-1));          // valor negativo
            Debug.Assert(false == t.IsNullOrEmpty(10.1));        // valor fracionado
            Debug.Assert(false == t.IsNullOrEmpty(10 + 10.1));   // cálculo

            Console.WriteLine("Is Null or Empty test finished!");        
        }

        public void Numbers()
        {
            // Não numéricos
            Debug.Assert(false == t.IsNumber(""));
            Debug.Assert(false == t.IsNumber(" "));
            Debug.Assert(false == t.IsNumber(null));
            Debug.Assert(false == t.IsNumber("A"));
            Debug.Assert(false == t.IsNumber("-"));
            Debug.Assert(false == t.IsNumber("+"));
            Debug.Assert(false == t.IsNumber(","));
            Debug.Assert(false == t.IsNumber("-,"));
            Debug.Assert(false == t.IsNumber("(10+10.1)"));     // expressão
            // Numéricos com erros
            Debug.Assert(false == t.IsNumber("0.1"));           // separador decimal != ','
            Debug.Assert(false == t.IsNumber(".1"));
            Debug.Assert(false == t.IsNumber("1."));
            Debug.Assert(false == t.IsNumber(",1"));            // erro digitação
            Debug.Assert(false == t.IsNumber("1,"));
            Debug.Assert(false == t.IsNumber("-1,"));
            Debug.Assert(false == t.IsNumber("-,1"));
            Debug.Assert(false == t.IsNumber("0,,1"));
            Debug.Assert(false == t.IsNumber("0,0,1"));
            Debug.Assert(false == t.IsNumber(",,1"));
            Debug.Assert(false == t.IsNumber("1 "));
            Debug.Assert(false == t.IsNumber(" 1"));
            Debug.Assert(false == t.IsNumber("1A"));
            Debug.Assert(false == t.IsNumber("0,A"));
            // String numéricas
            Debug.Assert(true == t.IsNumber("0"));
            Debug.Assert(true == t.IsNumber("1"));
            Debug.Assert(true == t.IsNumber("-1"));
            Debug.Assert(true == t.IsNumber("+1"));
            Debug.Assert(true == t.IsNumber("10"));
            Debug.Assert(true == t.IsNumber("0,1"));
            // Numéricos
            Debug.Assert(true == t.IsNumber(-10));
            Debug.Assert(true == t.IsNumber(10.1));
            Debug.Assert(true == t.IsNumber(10 + 10.1));

            Console.WriteLine("Numbers test finished!");
        }

        public void Validate()
        {
            // Nulo ou vazio
            Debug.Assert(false == t.Validate(""));
            Debug.Assert(false == t.Validate(" "));
            Debug.Assert(false == t.Validate(null));
            // Caracteres inválidos
            Debug.Assert(false == t.Validate("a"));
            Debug.Assert(false == t.Validate("(0,1+-,2/5.)"));  // separador decimal != ','
            Debug.Assert(false == t.Validate("(01,23+-,4*5,6/7-8+9,)+A"));
            // Parênteses sem par
            Debug.Assert(false == t.Validate("(((1+2))"));      // '(' > ')'
            Debug.Assert(false == t.Validate("(((01,23+-,4*5,6/7-8+9,))))"));
            // Somente caracteres válidos
            Debug.Assert(true == t.Validate("0"));
            Debug.Assert(true == t.Validate("-"));
            Debug.Assert(true == t.Validate("-1"));
            Debug.Assert(true == t.Validate("0+1"));
            Debug.Assert(true == t.Validate("+(-1)"));
            Debug.Assert(true == t.Validate("+ ( - 1 )"));

            Console.WriteLine("Validate test finished!");
        }

        public void Prepare()
        {
            // Soma de número negativo '+-'
            Debug.Assert("1-1" == t.Prepare("1+-1"));
            // Operador '+' junto ao '-'
            Debug.Assert("0-1" == t.Prepare("+-1"));
            // Operador '+' no início
            Debug.Assert("0+1-2" == t.Prepare("+1+-2"));
            // Operador '-' no início
            Debug.Assert("0-1-1" == t.Prepare("-1-1"));
            // Multiplicação de número negativo '*-'
            Debug.Assert("1*(0-1)*1" == t.Prepare("1*-1"));
            // Multiplicação de número positivo '*+'
            Debug.Assert("1*1" == t.Prepare("1*+1"));
            // Divisão de número negativo '*-'
            Debug.Assert("1*(0-1)/1" == t.Prepare("1/-1"));
            // Divisão de número negativo '*+'
            Debug.Assert("1/1" == t.Prepare("1/+1"));
            // Operador '-' próximo a parênteses
            Debug.Assert("0-(0-(0-15*(0-1)*10)))" == t.Prepare("-(-(-15*-10)))"));

            Console.WriteLine("Prepare test finished!");
        }

        public void PostFix()
        {
            //Inválido
            Debug.Assert("ERROR" == t.PostFix(""));
            Debug.Assert("ERROR" == t.PostFix(" "));
            Debug.Assert("ERROR" == t.PostFix(null));
            Debug.Assert("ERROR" == t.PostFix("expression"));
            //Número
            Debug.Assert("10" == t.PostFix(10));                //valor inteiro
            Debug.Assert("0" == t.PostFix("0"));
            Debug.Assert("0 1 -" == t.PostFix(-1));             //valor negativo
            Debug.Assert("0 1 -" == t.PostFix("-1"));
            Debug.Assert("0 0,1 -" == t.PostFix("-0,1"));       //valor fracionado
            //Expressão
            Debug.Assert("0 1 - 1 +" == t.PostFix("- 1 + 1"));  //espaço
            Debug.Assert("0 1 - 1 +" == t.PostFix("-1+1"));
            Debug.Assert("0 1 + 2 -" == t.PostFix("0+1-2"));
            Debug.Assert("1 2 + 3 +" == t.PostFix("1+2+3"));
            Debug.Assert("1,1 1,1 +" == t.PostFix("1,1+1,1"));
            Debug.Assert("1 1 1 + +" == t.PostFix("1+(1+1)"));
            Debug.Assert("1 0,1 + 2 +" == t.PostFix("(1+0,1)+2"));
            Debug.Assert("1 0,1 * 15 -" == t.PostFix("(1*0,1)-15"));
            Debug.Assert("1 0,1 / 1 -" == t.PostFix("(1/0,1)+-1"));
            Debug.Assert("1 0,1 / 0 1 - * 1 *" == t.PostFix("(1/0,1)*-1"));
            Debug.Assert("1 0,1 / 0 1 - * 1 * 15 +" == t.PostFix("(1/0,1)*-1+15"));
            Debug.Assert("1 0,1 / 0 1 - * 5 /" == t.PostFix("(1/0,1)/-5"));
            Debug.Assert("1 0,1 / 0 1 - 1 + /" == t.PostFix("(1/0,1)/(-1+1)"));
            Debug.Assert("1 1,1 - 0 5 - 1 + -" == t.PostFix("((1+-1,1))+-((-5+1))"));
            Debug.Assert("0 0 1 - - 0 2 - 3 + -" == t.PostFix("-(((-1)))+-(((-2+3)))"));
            Debug.Assert("0 0 1 - + 0 2 - 3 + -" == t.PostFix("+(((-1)))+-(((-2+3)))"));
            Debug.Assert("1,0 4,0 +" == t.PostFix("1,0+4,0"));
            Debug.Assert("1,0 4,0 + 2,0 + 3 +" == t.PostFix("1,0+4,0+2,0+3"));
            Debug.Assert("5,0 1,0 -" == t.PostFix("5,0-1,0"));
            Debug.Assert("5,0 2,0 - 2 -" == t.PostFix("5,0-2,0-2"));
            Debug.Assert("5,0 2,0 *" == t.PostFix("5,0*2,0"));
            Debug.Assert("5,0 2,0 * 2 *" == t.PostFix("5,0*2,0*2"));
            Debug.Assert("10,0 2,0 /" == t.PostFix("10,0/2,0"));
            Debug.Assert("10,0 2,0 / 2 / 10 /" == t.PostFix("10,0/2,0/2/10"));
            Debug.Assert("10 10 + 5 2 * -" == t.PostFix("(10+10)-(5*2)"));
            Debug.Assert("1,5 2,5 + 3 + 2,5 2,5 + -" == t.PostFix("(1,5+2,5+3)-(2,5+2,5)"));
            Debug.Assert("5 0 1 - * 10 * 20 + 5 -" == t.PostFix("5*-10+20-5"));
            Debug.Assert("2000 1 2 / +" == t.PostFix("2000+1/2"));
            Debug.Assert("2 2 + 4 5 * + 1 1000 / +" == t.PostFix("2+2+4*5+1/1000"));

            Console.WriteLine("PostFix test finished!");
        }

        public void Resolve()
        {
            Debug.Assert("ERROR" == t.Resolve(""));
            Debug.Assert("ERROR" == t.Resolve(" "));
            Debug.Assert("ERROR" == t.Resolve(null));
            Debug.Assert("ERROR" == t.Resolve("A"));
            Debug.Assert("ERROR" == t.Resolve(",1"));
            Debug.Assert("ERROR" == t.Resolve("(1 + 2)"));  // input: infix, esperado: posfix
            // Número
            Debug.Assert("0" == t.Resolve("0"));
            Debug.Assert("0" == t.Resolve(" 0"));
            Debug.Assert("1,1" == t.Resolve("1,1"));
            // Expressão
            Debug.Assert("0,25" == t.Resolve("10,0 2,0 / 2 / 10 /"));
            Debug.Assert("10" == t.Resolve("10 10 + 5 2 * -"));
            Debug.Assert("2" == t.Resolve("1,5 2,5 + 3 + 2,5 2,5 + -"));
            Debug.Assert("-1" == t.Resolve("0 1 -"));
            Debug.Assert("0" == t.Resolve("0 1 - 1 +"));
            Debug.Assert("-1" == t.Resolve("0 1 + 2 -"));
            Debug.Assert("6" == t.Resolve("1 2 + 3 +"));
            Debug.Assert("2,2" == t.Resolve("1,1 1,1 +"));
            Debug.Assert("3,1" == t.Resolve("1 0,1 + 2 +"));
            Debug.Assert("-14,9" == t.Resolve("1 0,1 * 15 -"));
            Debug.Assert("9" == t.Resolve("1 0,1 / 1 -"));
            Debug.Assert("-10" == t.Resolve("1 0,1 / 0 1 - * 1 *"));
            Debug.Assert("5" == t.Resolve("1 0,1 / 0 1 - * 1 * 15 +"));
            Debug.Assert("-2" == t.Resolve("1 0,1 / 0 1 - * 5 /"));
            Debug.Assert("ERROR" == t.Resolve("1 0,1 / 0 1 - 1 + /"));  // Divisão por zero
            Debug.Assert("3,9" == t.Resolve("1 1,1 - 0 5 - 1 + -"));
            Debug.Assert("0" == t.Resolve("0 0 1 - - 0 2 - 3 + -"));
            Debug.Assert("-2" == t.Resolve("0 0 1 - + 0 2 - 3 + -"));
            Debug.Assert("-35" == t.Resolve("5 0 1 - * 10 * 20 + 5 -"));
            Debug.Assert("2000,5" == t.Resolve("2000 1 2 / +"));
            Debug.Assert("24,001" == t.Resolve("2 2 + 4 5 * + 1 1000 / +"));
            Debug.Assert("2" == t.Resolve("1,5 2,5 + 3 + 2,5 2,5 + -"));
            Debug.Assert("757" == t.Resolve("100 200 + 2 / 5 * 7 +"));
            Debug.Assert("-4" == t.Resolve("2 3 1 * + 9 -"));
            Debug.Assert("23" == t.Resolve("10 2 8 * + 3 -"));

            Console.WriteLine("Resolve test finished!");
        }

        public void Calc()
        {
            Debug.Assert("0,0" == t.ExpressionCalc("0"));
            Debug.Assert("1,0" == t.ExpressionCalc("1"));
            Debug.Assert("-1,0" == t.ExpressionCalc("-1"));
            Debug.Assert("0,0" == t.ExpressionCalc("-1+1"));
            Debug.Assert("-1,0" == t.ExpressionCalc("0+1-2"));
            Debug.Assert("6,0" == t.ExpressionCalc("1+2+3"));
            Debug.Assert("2,2" == t.ExpressionCalc("1,1+1,1"));
            Debug.Assert("3,1" == t.ExpressionCalc("(1+0,1)+2"));
            Debug.Assert("-14,9" == t.ExpressionCalc("(1*0,1)-15"));
            Debug.Assert("9,0" == t.ExpressionCalc("(1/0,1)+-1"));
            Debug.Assert("-10,0" == t.ExpressionCalc("(1/0,1)*-1"));
            Debug.Assert("-10,0" == t.ExpressionCalc("(1/0,1)/-1"));
            Debug.Assert("ERROR" == t.ExpressionCalc("(1/0,1)/(-1+1)"));
            Debug.Assert("3,9" == t.ExpressionCalc("((1+-1,1))+-((-5+1))"));
            Debug.Assert("0,0" == t.ExpressionCalc("-(((-1)))+-(((-2+3)))"));
            Debug.Assert("-35,0" == t.ExpressionCalc("5*-10+20-5"));
            Debug.Assert("2000,5" == t.ExpressionCalc("2000+1/2"));
            Debug.Assert("24,001" == t.ExpressionCalc("2+2+4*5+1/1000"));
            Debug.Assert("23,0" == t.ExpressionCalc("((10 + (2 * 8)) - 3)"));

            Console.WriteLine("Calc test finished!");
        }

        public void Extra()
        {
            // Outro delimitador
            var postfix = t.PostFix("1+2*5,0+-1", '|');
            Debug.Assert(10 == Double.Parse(t.Resolve(postfix, '|')));

            Console.WriteLine("Extra test finished!");
        }
    }
}
