using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("root");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Functions());
            program.Children.Add(Main_function());
            MessageBox.Show("Success");
            return program;
        }
        Node Functions ()
        {
            Node functions  = new Node("Functions");
            if(TokenStream[InputPointer+1].lex!="main"&&(TokenStream[InputPointer].token_type==Token_Class.Tint|| TokenStream[InputPointer].token_type == Token_Class.Tfloat || TokenStream[InputPointer].token_type == Token_Class.Tstring))
            {
                functions.Children.Add(Function());
                functions.Children.Add(Functions());
                return functions;
            }
            return null;

        }
        Node Function()
        {
            Node function = new Node("function");
            function.Children.Add(Function_header());
            function.Children.Add(Function_body());
            return function;
        }

        Node Main_function()
        {
            Node main_function = new Node("Main_function");
            main_function.Children.Add(match(Token_Class.Tint));
            main_function.Children.Add(match(Token_Class.Idenifier));
            main_function.Children.Add(match(Token_Class.LParanthesis));
            main_function.Children.Add(match(Token_Class.RParanthesis));
            main_function.Children.Add(Function_body());
            return main_function;
        }
        Node Function_header()
        {
            Node function_header = new Node("Function_header");
            function_header.Children.Add(Datatype());
            function_header.Children.Add(match(Token_Class.Idenifier));
            function_header.Children.Add(match(Token_Class.LParanthesis));
            function_header.Children.Add(ParameterList());
            function_header.Children.Add(match(Token_Class.RParanthesis));
            return function_header;
        }
        Node Function_body()
        {
            Node function_body = new Node("Function_body");
            function_body.Children.Add(match(Token_Class.openCurlybrac));
            function_body.Children.Add(Statements());
            function_body.Children.Add(Return_statement());
            function_body.Children.Add(match(Token_Class.closeCurlybrac));
            return function_body;
        }
        Node ParameterList()
        {
            Node parameterList = new Node("ParameterList");
            if (TokenStream[InputPointer].token_type == Token_Class.Tint || TokenStream[InputPointer].token_type == Token_Class.Tfloat || TokenStream[InputPointer].token_type == Token_Class.Tstring)
            {
                parameterList.Children.Add(Parameters());
                return parameterList;
            }
            return null;
        }

        Node Parameters()
        {
            Node parameters = new Node("Parameters");
            parameters.Children.Add(Datatype());
            parameters.Children.Add(match(Token_Class.Idenifier));
            parameters.Children.Add(Parameter());
            return parameters;
        }
        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            if(TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                parameter.Children.Add(match(Token_Class.Comma));
                parameter.Children.Add(Datatype());
                parameter.Children.Add(match(Token_Class.Idenifier));
                parameter.Children.Add(Parameter());
                return parameter;
            }
            return null;
        }

        Node Statements()
        {
            Node statements = new Node("Statements");
            statements.Children.Add(Statement());
            statements.Children.Add(State());
            return statements;
        }
        
        Node State()
        {
            Node state = new Node("State");
            if(TokenStream[InputPointer].token_type == Token_Class.Semicolon)
            {
                state.Children.Add(match(Token_Class.Semicolon));
                state.Children.Add(Statement());
                state.Children.Add(State());
                return state;
            }
            return null;
        }

        Node Statement()
        {
            
            Node statement = new Node("Statement");
            if(TokenStream[InputPointer].token_type == Token_Class.Tread)
            {
                statement.Children.Add(Read_Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Trepeat)
            {
                statement.Children.Add(Repeat_Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Tif)
            {
                statement.Children.Add(If_Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Twrite)
            {
                statement.Children.Add(Write_Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Tint || TokenStream[InputPointer].token_type == Token_Class.Tfloat || TokenStream[InputPointer].token_type == Token_Class.Tstring)
            {
                statement.Children.Add(Declaration_Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if(TokenStream[InputPointer+1].token_type == Token_Class.assignPp)
                {
                    statement.Children.Add(Assignment_Statement());
                    return statement;
                }
                else
                {
                    statement.Children.Add(Function_Call());
                    return statement;
                }
            }
            return null;
        }

        Node Write_term()
        {
            Node write_term = new Node("Write_term");
            if (TokenStream[InputPointer].token_type == Token_Class.Tendl)
                write_term.Children.Add(match(Token_Class.Tendl));
            else
                write_term.Children.Add(Expression());
            return write_term;
        }
        Node Repeat_Statement()
        {
            Node repeat_Statement = new Node("Repeat_Statement");
            repeat_Statement.Children.Add(match(Token_Class.Trepeat));
            repeat_Statement.Children.Add(Statements());
            repeat_Statement.Children.Add(match(Token_Class.Tuntil));
            repeat_Statement.Children.Add(Condition_Statement());
            return repeat_Statement;
        }
        Node Else_Statement()
        {
            Node else_Statement = new Node("Else_Statement");
            else_Statement.Children.Add(match(Token_Class.Telse));
            else_Statement.Children.Add(Statements());
            else_Statement.Children.Add(match(Token_Class.Tend));
            return else_Statement;
        }
        Node ElseIf_Statement()
        {
            Node elseIf_Statement = new Node("ElseIf_Statement");

            elseIf_Statement.Children.Add(match(Token_Class.Telseif));
            elseIf_Statement.Children.Add(Condition_Statement());
            elseIf_Statement.Children.Add(match(Token_Class.Tthen));
            elseIf_Statement.Children.Add(Statements());
            elseIf_Statement.Children.Add(ElseOrElseif_Statement());
            return elseIf_Statement;
        }
        Node ElseOrElseif_Statement()
        {
            Node elseOrElseif_Statement = new Node("ElseOrElseif_Statement");
            if(TokenStream[InputPointer].token_type == Token_Class.Telseif)
            {
                elseOrElseif_Statement.Children.Add(ElseIf_Statement());
                return elseOrElseif_Statement;
            }
                
            else if(TokenStream[InputPointer].token_type == Token_Class.Telse)
            {
                elseOrElseif_Statement.Children.Add(Else_Statement());
                return elseOrElseif_Statement;
            }
            return null;
        }
        Node If_Statement()
        {
            Node if_Statement = new Node("If_Statement");
            if_Statement.Children.Add(match(Token_Class.Tif));
            if_Statement.Children.Add(Condition_Statement());
            if_Statement.Children.Add(match(Token_Class.Tthen));
            if_Statement.Children.Add(Statements());
            if_Statement.Children.Add(ElseOrElseif_Statement());
            return if_Statement;
        }
        Node Condition_Statement()
        {
            Node condition_Statement = new Node("Condition_Statement");
            condition_Statement.Children.Add(Condition());
            if (TokenStream[InputPointer].token_type == Token_Class.and|| TokenStream[InputPointer].token_type == Token_Class.or)
            {
                condition_Statement.Children.Add(Boolean_Operator());
                condition_Statement.Children.Add(Condition_Statement());
            }
            return condition_Statement;
        }
        Node Boolean_Operator()
        {
            Node boolean_Operator = new Node("Boolean_Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.and)
                boolean_Operator.Children.Add(match(Token_Class.and));
            else if(TokenStream[InputPointer].token_type == Token_Class.or)
                boolean_Operator.Children.Add(match(Token_Class.or));
            return boolean_Operator;
        }
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());
            return condition;
        }
        Node Condition_Operator()
        {
            Node condition_Operator = new Node("Condition_Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
                condition_Operator.Children.Add(match(Token_Class.LessThanOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
                condition_Operator.Children.Add(match(Token_Class.GreaterThanOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.lessorequal)
                condition_Operator.Children.Add(match(Token_Class.lessorequal));

            else if (TokenStream[InputPointer].token_type == Token_Class.greaterorequal)
                condition_Operator.Children.Add(match(Token_Class.greaterorequal));

            else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
                condition_Operator.Children.Add(match(Token_Class.EqualOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
                condition_Operator.Children.Add(match(Token_Class.NotEqualOp));
            return condition_Operator;
        }
        Node Return_statement()
        {
            Node return_statement = new Node("Return_statement");
            return_statement.Children.Add(match(Token_Class.Treturn));
            return_statement.Children.Add(Expression());
            return_statement.Children.Add(match(Token_Class.Semicolon));
            return return_statement;
        }

        Node Read_Statement()
        {
            Node read_Statement = new Node("Read_Statement");
            read_Statement.Children.Add(match(Token_Class.Tread));
            read_Statement.Children.Add(match(Token_Class.Idenifier));
            return read_Statement;
        }

        Node Write_Statement()
        {
            Node write_Statement = new Node("Write_Statement");
            write_Statement.Children.Add(match(Token_Class.Twrite));
            write_Statement.Children.Add(Write_term());
            return write_Statement;
        }

        Node Declaration_Statement()
        {
            Node declaration_Statement = new Node("Declaration_Statement");
            declaration_Statement.Children.Add(Datatype());
            declaration_Statement.Children.Add(Vars());
            return declaration_Statement;
        }


        Node Vars()
        {
            Node vars = new Node("Vars");
            vars.Children.Add(VarDeclare());
            vars.Children.Add(Variable());
            return vars;
        }
        Node Variable()
        {
            Node variable = new Node("Variable");
            if(TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                variable.Children.Add(match(Token_Class.Comma));
                variable.Children.Add(VarDeclare());
                variable.Children.Add(Variable());
                return variable;
            }
            return null;
        }
        Node VarDeclare()
        {
            Node varDeclare = new Node("VarDeclare");
            if (TokenStream[InputPointer + 1].token_type == Token_Class.assignPp)
            {
                varDeclare.Children.Add(Assignment_Statement());
            }
            else
                varDeclare.Children.Add(match(Token_Class.Idenifier));
            return varDeclare;
        }
        Node Datatype()
        {
            Node datatype = new Node("Datatype");
            if (TokenStream[InputPointer].token_type == Token_Class.Tint)
                datatype.Children.Add(match(Token_Class.Tint));
            else if(TokenStream[InputPointer].token_type == Token_Class.Tfloat)
                datatype.Children.Add(match(Token_Class.Tfloat));
            else if (TokenStream[InputPointer].token_type == Token_Class.Tstring)
                datatype.Children.Add(match(Token_Class.Tstring));
            return datatype;
        }
        
        Node Assignment_Statement()
        {
            Node assignment_Statement = new Node("Assignment_Statement");
            assignment_Statement.Children.Add(match(Token_Class.Idenifier));
            assignment_Statement.Children.Add(match(Token_Class.assignPp));
            assignment_Statement.Children.Add(Expression());
            return assignment_Statement;
        }
        Node Expression()
        {

            bool a = TokenStream[InputPointer + 1].token_type != Token_Class.PlusOp;
            bool b = TokenStream[InputPointer + 1].token_type != Token_Class.MinusOp;
            bool c = TokenStream[InputPointer + 1].token_type != Token_Class.MultiplyOp;
            bool d = TokenStream[InputPointer + 1].token_type != Token_Class.DivideOp;
            Node expression = new Node("Expression");
            if (TokenStream[InputPointer].token_type == Token_Class.stringVal)
                expression.Children.Add(match(Token_Class.stringVal));
            else if((TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.Idenifier)&&(a&&b&&c&&d))
                expression.Children.Add(Term());
            else
                expression.Children.Add(Equation());
            return expression;

        }
        Node Equation()
        {
            Node equation = new Node("Equation");
            if(TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                equation.Children.Add(match(Token_Class.LParanthesis));
                equation.Children.Add(Equation());
                equation.Children.Add(match(Token_Class.RParanthesis));
            }
            else
            {
                equation.Children.Add(Term());
                bool a = TokenStream[InputPointer].token_type == Token_Class.PlusOp;
                bool b = TokenStream[InputPointer].token_type == Token_Class.MinusOp;
                bool c = TokenStream[InputPointer].token_type == Token_Class.MultiplyOp;
                bool d = TokenStream[InputPointer].token_type == Token_Class.DivideOp;
                if (a || b || c || d)
                {
                    equation.Children.Add(Arithmatic_Operator());
                    equation.Children.Add(Equation());

                }
            }
            return equation;
        }

        Node Arithmatic_Operator()
        {
            Node arithmatic_Operator = new Node("Arithmatic_Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                arithmatic_Operator.Children.Add(match(Token_Class.PlusOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                arithmatic_Operator.Children.Add(match(Token_Class.MinusOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                arithmatic_Operator.Children.Add(match(Token_Class.MultiplyOp));

            else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                arithmatic_Operator.Children.Add(match(Token_Class.DivideOp));

            return arithmatic_Operator;

        }

        Node Term()
        {
            Node term = new Node("Term");
            if (TokenStream[InputPointer].token_type == Token_Class.Number)
                term.Children.Add(match(Token_Class.Number));
            else if (TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                term.Children.Add(Function_Call());
            else
                term.Children.Add(match(Token_Class.Idenifier));
            return term;
        }

        Node Function_Call()
        {
            Node function_Call = new Node("Function_Call");
            function_Call.Children.Add(match(Token_Class.Idenifier));
            function_Call.Children.Add(match(Token_Class.LParanthesis));
            function_Call.Children.Add(Arguments());
            function_Call.Children.Add(match(Token_Class.RParanthesis));
            return function_Call;
        }
        Node Arguments()
        {
            Node arguments = new Node("Arguments");
            arguments.Children.Add(match(Token_Class.Idenifier));
            arguments.Children.Add(Arg());
            return arguments;
        }

        Node Arg()
        {
            Node arg = new Node("Arg");
            if(TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                arg.Children.Add(match(Token_Class.Comma));
                arg.Children.Add(match(Token_Class.Idenifier));
                arg.Children.Add(Arg());
                return arg;
            }
            return null;
           
        }

        // Implement your logic here


        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
