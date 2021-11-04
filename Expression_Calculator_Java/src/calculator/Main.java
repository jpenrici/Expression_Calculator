package calculator;

public class Main {

    public static void main(String[] args) {

        if (args.length == 1) {
            if (args[0].equals("--test")) {
                MainTest.test();
            } else {
                Calculator calc = new Calculator();
                System.out.println(calc.expressionCalc(args[0]));
            }
        } else {
            System.out.println("Nothing to do.");
        }
    }
}
