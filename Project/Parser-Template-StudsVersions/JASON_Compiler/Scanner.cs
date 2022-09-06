using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

public enum Token_Class
{
    Tint , Tfloat , Tstring , Tread , Twrite , Trepeat , Tuntil , Tif , Telseif , Telse , Tthen , Treturn , Tendl,
    Tend,Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,and,or,
    Idenifier , assignPp , openCurlybrac , closeCurlybrac,Number,stringVal,greaterorequal,lessorequal
}
namespace JASON_Compiler
{
    

    public class Token
    {
       public string lex;
       public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.Tif);
            ReservedWords.Add("end", Token_Class.Tend);
            ReservedWords.Add("else", Token_Class.Telse);
            ReservedWords.Add("int", Token_Class.Tint);
            ReservedWords.Add("read", Token_Class.Tread);
            ReservedWords.Add("float", Token_Class.Tfloat);
            ReservedWords.Add("then", Token_Class.Tthen);
            ReservedWords.Add("until", Token_Class.Tuntil);
            ReservedWords.Add("repeat", Token_Class.Trepeat);
            ReservedWords.Add("write", Token_Class.Twrite);
            ReservedWords.Add("string", Token_Class.Tstring);
            ReservedWords.Add("elseif", Token_Class.Telseif);
            ReservedWords.Add("return", Token_Class.Treturn);
            ReservedWords.Add("endl", Token_Class.Tendl);
            ReservedWords.Add("Number", Token_Class.Number);



            Operators.Add(":=", Token_Class.assignPp);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("{", Token_Class.openCurlybrac);
            Operators.Add("}", Token_Class.closeCurlybrac);
            Operators.Add("<=", Token_Class.lessorequal);
            Operators.Add(">=", Token_Class.greaterorequal);
            Operators.Add("||", Token_Class.or);
            Operators.Add("&&", Token_Class.and);

        }

    public bool is_Letter(char c)
        {
            if(c >= 'A' && c <= 'z')
            {
                return true;
            }
            return false;
        }

    public bool is_Digit(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return true;
            }
            return false;
        }
        public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\t' || CurrentChar == '\n')
                    continue;

                else if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    j++;
                    while(j<SourceCode.Length&&(is_Letter(SourceCode[j])|| is_Digit( SourceCode[j])))
                    {
                        
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j-1;
                }

                else if(CurrentChar >= '0' && CurrentChar <= '9')
                {
                    j++;
                    while (j < SourceCode.Length && is_Digit(SourceCode[j]))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    if (SourceCode[j] == '.')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                        while (j < SourceCode.Length && is_Digit(SourceCode[j]))
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else if(CurrentChar == '/'&&SourceCode[i+1]=='*')
                {
                    bool finish = false;
                    CurrentLexeme += SourceCode[j+1];
                    j += 2;
                   while(j < SourceCode.Length)
                    {
                        CurrentLexeme += SourceCode[j];
                        if (j + 1 < SourceCode.Length && SourceCode[j] == '*' && SourceCode[j + 1] == '/')
                        {
                            finish = true;
                            break;
                        }
                            j++;
                    }
                    j++;
                    if (!finish) FindTokenClass(CurrentLexeme);
                    
                    i = j;
                    
                }

                else if (CurrentChar == '\"' )
                {
                    j++;
                    while (j  < SourceCode.Length && SourceCode[j] != '\"' )
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    CurrentLexeme += SourceCode[j];
                    FindTokenClass(CurrentLexeme);
                    i = j ;
                }
                else
                {
                 if ((j + 1 < SourceCode.Length)&&(SourceCode[j+1]=='=' || SourceCode[j + 1] == '>' || SourceCode[j + 1] == '|' || SourceCode[j + 1] == '&')) {
                        CurrentLexeme += SourceCode[j+1];
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
            }
            
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Console.WriteLine(Lex);
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                TC = Token_Class.Idenifier;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }

            //Is it a number?
            else if (isNumber(Lex))
            {
                TC = Token_Class.Number;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            else if (isString(Lex))
            {
                TC = Token_Class.stringVal;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }

            //Is it an operator?
           else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }

            //Is it an undefined?
            else
            {
                Errors.Error_List.Add("Undefined : "+Lex);
            }
        }


        bool isString(string lex)
        {
            // Check if the lex is an identifier or not.
            Regex rx = new Regex("^\"([a-zA-Z0-9]|[^0])*\"$");
            return rx.IsMatch(lex);
        }
        bool isIdentifier(string lex)
        {
            // Check if the lex is an identifier or not.
            Regex rx = new Regex(@"^[a-zA-Z]([a-zA-Z]|[0-9]|'_')*$");
            return rx.IsMatch(lex);
        }
        bool isNumber(string lex)
        {
            // Check if the lex is a constant (Number) or not.
            Regex rx = new Regex(@"^[0-9]+(\.[0-9]+)?$");
            return rx.IsMatch(lex);
        }
    }
}
