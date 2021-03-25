using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace EquationEvaluator
{
    class Evaluator
    {
        public dynamic solve(string equation)
        {
            string[] eq1 = simplify(equation.Split('=')[0]);
            string[] eq2 = simplify(equation.Split('=')[1]);

            if (findVariable(eq1[0])[0] != findVariable(eq2[0])[0]) return "Only supports equations with one variable";
            string variable = findVariable(eq1[0])[0];

            double denominatorVal1 = Convert.ToDouble(eq1[0].Replace(findVariable(eq1[0])[0], ""));
            double denominatorVal2 = Convert.ToDouble(eq2[0].Replace(findVariable(eq2[0])[0], ""));

            double num1 = Convert.ToDouble(eq1[1]);
            double num2 = Convert.ToDouble(eq2[1]);

            if (denominatorVal1 == denominatorVal2)
            {
                if (num1 == num2) return "Solution: All real values";
                else return "No solution for real values";
            }

            if (num1 == num2)
            {
                if (denominatorVal1 == denominatorVal2) return "Solution: All real values";
                else return "No solution for real values";
            }

            //
            double[] finaleq = { 0.0, 0.0 }; // index 0 for denominator of variable and index 1 for whole number
            finaleq[0] = denominatorVal1 - denominatorVal2;
            finaleq[1] = num1 - num2;

            double result = finaleq[1] / finaleq[0];

            return $"{variable} = {result}";
        }

        dynamic simplify(string context)
        {
            if (findVariable(context).Length > 1)
            {
                Console.WriteLine("Currently only supporting equations with one variable");
                return null;
            }
            string variable = findVariable(context)[0];

            MSScriptControl.ScriptControl sc = new(); // For computing string math
            sc.Language = "VBScript";

            if (context.ToCharArray()[0] == '-')
            {
                context = "0" + context; // eg. 12x+5 -> 0+12x+5 because code reads backwards from variable to minus or plus operator 
            }
            else
            {
                context = "0+" + context;
            }

            char[] args = context.ToCharArray();
            List<int> varPos = new List<int>();
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i].ToString() == variable) varPos.Add(i);
            }

            List<string> fullvars = new List<string>();
            List<string> fullvarsRaw = new List<string>();
            for (int n = 0; n < varPos.Count; n++)
            {
                int pos;
                List<string> arr = new List<string>();
                for (int i = varPos[n]-1; i > -1; i--)
                {
                    if (args[i] != '+' && args[i] != '-')
                    {
                        arr.Add(args[i].ToString());
                        //Console.WriteLine(args[i]); //Debug
                    }
                    else
                    {
                        //Console.WriteLine("Out of if"); //Debug

                        pos = i;
                        string str = "";
                        arr.Reverse();
                        for (int k = 0; k < arr.Count; k++) str += arr[k];
                        //Console.WriteLine(str); //Debug

                        fullvarsRaw.Add(args[pos] + str + variable);

                        if (str == "") str = "1"; // To compute "x" as "1x"

                        string res = sc.Eval(str).ToString();
                        res = res.Replace(",", ".");

                        string fullstr = args[pos] + res + variable;
                        fullvars.Add(fullstr);

                        break;
                    }
                }
            }

            string trimmed = context;
            for(int i=0;i<fullvarsRaw.Count;i++) trimmed = trimmed.Replace(fullvarsRaw[i], "");

            string denominator = "";
            for(int i = 0; i < fullvars.Count; i++)
            {
                denominator += fullvars[i].Replace(variable, "");
            }

            if (trimmed == "") trimmed = "0";

            //Console.WriteLine(trimmed); // Debug

            trimmed = trimmed.Replace(",", ".");
            string result = sc.Eval(trimmed).ToString();

            denominator = denominator.Replace(",", ".");
            string denominatorResult = sc.Eval(denominator).ToString() + variable;

            List<string> fullres = new List<string>();
            if (result.ToCharArray()[0] == '-')
            {
                fullres.Add(denominatorResult); 
                fullres.Add(result);
            }
            else
            {
                fullres.Add(denominatorResult);
                fullres.Add("+"+result);
            }

            Console.WriteLine($"\t{fullres[0]}{fullres[1]}");

            return fullres.ToArray();
        }

        static string[] findVariable(string context)
        {
            List<string> variables = new List<string>();

            char[] args = context.ToCharArray();
            for(int i = 0; i < args.Length; i++)
            {
                if (Char.IsLetter(Convert.ToChar(args[i])))
                {
                    variables.Add(args[i].ToString());
                }
            }

            var result = variables.ToArray().Distinct();
            variables = new List<string>();
            foreach(string value in result)
            {
                variables.Add(value);
            }

            return variables.ToArray();
        }
    }
}
