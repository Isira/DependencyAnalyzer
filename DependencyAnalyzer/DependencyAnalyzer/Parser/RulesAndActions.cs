///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.1                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu    //
// Source:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 3.0 : 06 Oct 2014
 * - Added Rules for Detecting Relationships(Aggregation, Inheritance, Composition and Using) 
 *   among user defined types.
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Planned Modifications (not needed for Project #2):
 * --------------------------------------------------
 * - add folding rules:
 *   - CSemiExp returns for(int i=0; i<len; ++i) { as three semi-expressions, e.g.:
 *       for(int i=0;
 *       i<len;
 *       ++i) {
 *     The first folding rule folds these three semi-expression into one,
 *     passed to parser. 
 *   - CToker returns operator[]( as four distinct tokens, e.g.: operator, [, ], (.
 *     The second folding rule coalesces the first three into one token so we get:
 *     operator[], ( 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DependencyAnalyzer;
namespace CodeAnalysis
{
  public class Elem  // holds scope information
  {
    public string type { get; set; }
    public string name { get; set; }
    public int begin { get; set; }
    public int end { get; set; }

    public override string ToString()
    {
      StringBuilder temp = new StringBuilder();
      temp.Append("{");
      temp.Append(String.Format("{0,-10}", type)).Append(" : ");
      temp.Append(String.Format("{0,-10}", name)).Append(" : ");
      temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
      temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
      temp.Append("}");
      return temp.ToString();
    }
  }
    
   // used to store results in one file parse
   public class  Repository
  {
    ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
    List<TypeElement> userDefinedTypes_ = new List<TypeElement>();
    RelationshipTable relationshipResults_ = new RelationshipTable();

    static Repository instance;

    public Repository()
    {
      instance = this;
    }

    public static Repository getInstance()
    {
      return instance;
    }
    // provides all actions access to current semiExp

    public CSsemi.CSemiExp semi
    {
      get;
      set;
    }

    // semi gets line count from toker who counts lines
    // while reading from its source

    public int lineCount  // saved by newline rule's action
    {
      get { return semi.lineCount; }
    }
    public int prevLineCount  // not used in this demo
    {
      get;
      set;
    }
    // enables recursively tracking entry and exit from scopes

    public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
    {
      get { return stack_; } 
    }
    // the locations table is the result returned by parser's actions
    // in this demo



    public List<TypeElement> userDefinedTypes
    {
        get { return userDefinedTypes_; }

    }

    public RelationshipTable relationshipTable
    {
        get { return relationshipResults_; }

    }
  }


  /////////////////////////////////////////////////////////
  // pushes scope info on stack when entering new scope
  public class PushStack : AAction
  {
    Repository repo_;

    public PushStack(Repository repo)
    {
      repo_ = repo;
    }
    public override void doAction(CSsemi.CSemiExp semi)
    {
      Elem elem = new Elem();
      elem.type = semi[0];  // expects type
      elem.name = semi[1];  // expects name
      elem.begin = repo_.semi.lineCount - 1;
      elem.end = 0;

      repo_.stack.push(elem);    
    }

   

  }

 

  /////////////////////////////////////////////////////////
  // pops scope info from stack when leaving scope
  public class PopStack : AAction
  {
    Repository repo_;

    public PopStack(Repository repo)
    {
      repo_ = repo;
    }
    public override void doAction(CSsemi.CSemiExp semi)
    {
      Elem elem;
      try
      {
        elem = repo_.stack.pop();
      }
      catch
      {
        Console.Write("popped empty stack on semiExp: ");
        semi.display();
        return;
      }
      CSsemi.CSemiExp local = new CSsemi.CSemiExp();
      local.Add(elem.type).Add(elem.name);
      if(local[0] == "control")
        return;

    }

  }

  ///////////////////////////////////////////////////////////
  // action which is called when relationship detected, Add
  // RelationshipResult to the repository
  public class AddRelationship : AAction
  {
      Repository repo_;
      TypeTable interestedTypes;
      TypeTable allLocallyDeclaredTypes;

      public AddRelationship(Repository repo, TypeTable _interestedTypes, TypeTable _allLocallyDeclaredTypes)
      {
          repo_ = repo;
          interestedTypes = _interestedTypes;
          allLocallyDeclaredTypes = _allLocallyDeclaredTypes;
      }
      public override void doAction(CSsemi.CSemiExp semi)
      {

          if (TryActionOnInheritance(semi))
              return;

          if(TryActionOnUsing(semi))
                return;
          if (! proceed(semi))
              return;

          if(TryActionOnComposition(semi))
                return;
          TryActionOnAggregation(semi);
         // Get class
 
      }

      private void TryActionOnAggregation(CSsemi.CSemiExp semi)
      {
          if (isInterestedType(semi[1]))
          {
              string relationship = semi[0];
              TypeElement parent = allLocallyDeclaredTypes.GetTypeElement(getContainingClass());
              TypeElement child = interestedTypes.GetTypeElement(semi[1]);
              repo_.relationshipTable.add(parent, child); ;
          }
      }

      private bool TryActionOnComposition(CSsemi.CSemiExp semi)
      {
          string type = semi[0];
          if (type == "Composition")
          {
              if (isInterestedStructOrEnum(semi[1]))
              {
                  string relationship = semi[0];
                  TypeElement parent = allLocallyDeclaredTypes.GetTypeElement(getContainingClass());
                  TypeElement child = interestedTypes.GetTypeElement(semi[1]);
                  repo_.relationshipTable.add(parent, child); 
              }

              return true;
          }
          return false;
      }

      private bool TryActionOnUsing(CSsemi.CSemiExp semi)
      {
          string type = semi[0];
          if (type == "function")
          {
              for (int i = 2; i < semi.count; i += 2)
              {
                  if (isInterestedType(semi[i + 1]))
                  {
                      string relationship = semi[i];
                      TypeElement parent = allLocallyDeclaredTypes.GetTypeElement(getContainingClass());
                      TypeElement child = interestedTypes.GetTypeElement(semi[i + 1]);
                      repo_.relationshipTable.add(parent, child); 
                  }

              }
              return true;

          }
          return false;
      }

      private bool TryActionOnInheritance(CSsemi.CSemiExp semi )
      {
          string type = semi[0];
          if (type == "class" || type == "interface")
          {
              for (int i = 2; i < semi.count; i += 2)
              {
                  if (isInterestedType(semi[i + 1]))
                  {
                      string relationship = semi[i];
                      TypeElement parent = allLocallyDeclaredTypes.GetTypeElement(getContainingClass());
                      TypeElement child = interestedTypes.GetTypeElement(semi[i + 1]);
                      repo_.relationshipTable.add(parent, child); 
                  }

              }
          }
          return false;
      }

      bool proceed(CSsemi.CSemiExp semi)
      {
          string type = semi[0];
          return (type == "Aggregate" || type == "Inherit" || type == "Composition" || type == "Using");
          
      }

      bool isInterestedType(string className) {
          return interestedTypes.types.ContainsKey(className);
      }

      bool isInterestedStructOrEnum(string className)
      {
          if(isInterestedType(className))
          {
              List<TypeElement> elements = interestedTypes.types[className];
              // Go throuth the List to find the correct element
              if(elements[0].Type == "struct" ||elements[0].Type == "enum")
              return true;
          }
          return false;
      }

      string getContainingClass() 
      {
          int size  = repo_.stack.count;
          string containingClass = "";
          for (int i = size - 1; i >= 0;i-- )
          {
              Elem elem = repo_.stack[i];
              if (elem.type == "class" || elem.type == "interface")
              {
                  containingClass = elem.name;
                  break;
              }
          }
          return containingClass;
      }



  }

  ///////////////////////////////////////////////////////////
  // action which is called when user defined type is detected, Add
  // the Elem object to the repository
  public class AddUserDefinedType : AAction
  {
      Repository repo_;

      public AddUserDefinedType(Repository repo)
      {
          repo_ = repo;
      }
      public override void doAction(CSsemi.CSemiExp semi)
      {
          TypeElement elem = new TypeElement();
          elem.Type = semi[0];  // expects type
          elem.TypeName = semi[1];  // expects name
          repo_.userDefinedTypes.Add(elem);
      }

  }

  ////////////////////////////////
  // rule to dectect namespaces
  public class DetectNamespace : ARule
  {
    public override bool test(CSsemi.CSemiExp semi)
    {
      int index = semi.Contains("namespace");
      if (index != -1)
      {
        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
        // create local semiExp with tokens for type and name
        local.displayNewLines = false;
        local.Add(semi[index]).Add(semi[index + 1]);
        doActions(local);
        return true;
      }
      return false;
    }
  }

  ////////////////////////////////////////////////////////////////////
  // rule to dectect user defined Type class, interface, enum or struct
  public class DetectType : ARule
  {
    public override bool test(CSsemi.CSemiExp semi)
    {
      int indexCL = semi.Contains("class");
      int indexIF = semi.Contains("interface");
      int indexST = semi.Contains("struct");
      int indexEN = semi.Contains("enum");

      int index = Math.Max(indexCL, indexIF);
      index = Math.Max(index, indexST);
      index = Math.Max(index,indexEN);
      if (index != -1)
      {
        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
        // local semiExp with tokens for type and name
        local.displayNewLines = false;
        local.Add(semi[index]).Add(semi[index + 1]);
        doActions(local);
        return true;
      }
      return false;
    }
  }
  
  /////////////////////////////////////////////////////////
  // rule to dectect function definitions
  public class DetectFunction : ARule
  {
    public static bool isSpecialToken(string token)
    {
      string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
      foreach (string stoken in SpecialToken)
        if (stoken == token)
          return true;
      return false;
    }
    public override bool test(CSsemi.CSemiExp semi)
    {
      if (semi[semi.count - 1] != "{")
        return false;

      int index = semi.FindFirst("(");
      if (index > 0 && !isSpecialToken(semi[index - 1]))
      {
        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
        local.Add("function").Add(semi[index - 1]);
        doActions(local);
        return true;
      }
      return false;
    }
  }
  
  /////////////////////////////////////////////////////////
  // detect entering anonymous scope
  // - expects namespace, class, and function scopes
  //   already handled, so put this rule after those
  public class DetectAnonymousScope : ARule
  {
    public override bool test(CSsemi.CSemiExp semi)
    {
      int index = semi.Contains("{");
      if (index != -1)
      {
        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
        // create local semiExp with tokens for type and name
        local.displayNewLines = false;
        local.Add("control").Add("anonymous");
        doActions(local);
        return true;
      }
      return false;
    }
  }
  
  /////////////////////////////////////////////////////////
  // detect leaving scope
  public class DetectLeavingScope : ARule
  {
    public override bool test(CSsemi.CSemiExp semi)
    {
      int index = semi.Contains("}");
      if (index != -1)
      {
        doActions(semi);
        return true;
      }
      return false;
    }
  }

  /////////////////////////////////////////////////////////
  // detect bracesless scope 
  public class DetectBracelessScope : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int brIndex = semi.Contains("{");

          if (brIndex != -1)
              return false;
          int hashIndex = semi.Contains("#");
          if (hashIndex != -1)
              return false;

          int index = semi.FindFirst("(");
          if (index > 0 && DetectFunction.isSpecialToken(semi[index - 1]))
          {
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              // create local semiExp with tokens for type and name
              local.displayNewLines = false;
              local.Add("control").Add("anonymous");
              doActions(local);
              return true;
          }
          return false;
      }
  }
  
 /////////////////////////////////////////////////////////////////
  // Detect Agregation, return true whenever new keyword is encountered
  public class DetectAggregation : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int openBrace = semi.Contains(";");
          if (openBrace == -1)
              return false;
          int index = semi.FindFirst("new");
       
          if (index > 0)
          {
              string className = semi[index + 1];
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              local.Add("Aggregate").Add(className);
              doActions(local);
              return true;
          }
          return false;
      }
  }

  /////////////////////////////////////////////////////////////////
  // Improved DetectType Rule to detect Inheritance
  public class DetectInheritance : ARule
  {
  public override bool test(CSsemi.CSemiExp semi)
   {
       int indexCL = semi.Contains("class");
       int indexIF = semi.Contains("interface");
       int indexST = semi.Contains("struct");
       int indexEN = semi.Contains("enum");

       int index = Math.Max(indexCL, indexIF);
       index = Math.Max(index, indexST);
       index = Math.Max(index, indexEN);
       if (index != -1)
       {
           CSsemi.CSemiExp local = new CSsemi.CSemiExp();
           local.Add(semi[index]).Add(semi[index + 1]);
           int colonToken = semi.Contains(":");
           if (colonToken != -1) 
           {
               local.Add("Inherit").Add(semi[colonToken + 1]);
               for (int i = colonToken + 3; i < semi.count - 1; i+=2) {

                   local.Add("Inherit").Add(semi[i]);                
               }
   
           }
          
           doActions(local);
           return true;
       }
       return false;
      }
  }
  
  /////////////////////////////////////////////////////////////////
  // Improved DetectFunction Rule to detect Using relationship by 
  //analyzing the function parameters
   public class DetectUsing : ARule {
      public static bool isSpecialToken(string token)
      {
          string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
          foreach (string stoken in SpecialToken)
              if (stoken == token)
                  return true;
          return false;
      }

      public override bool test(CSsemi.CSemiExp semi)
      {
   
          if (semi[semi.count - 1] != "{")
              return false;
          int indexOpen = semi.FindFirst("(");

          if (indexOpen > 0 && !isSpecialToken(semi[indexOpen - 1]))
          {
              int indexClose = semi.FindFirst(")");
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              local.Add("function").Add(semi[indexOpen - 1]);

              for (int i = indexOpen+1; i < indexClose;i++ ) 
              {
                  // Handle When out ref keywords are used
                  // This assumes that the Type name resides just after the open bracket or ,
                  if ((semi[i-1] =="(" || semi[i-1] == ",")) {
                      string usedType;
                      if(isParameterModifier(semi[i]))
                            usedType = semi[i+1];
                      else
                          usedType = semi[i];
                    
                      local.Add("Using").Add(usedType);
                     
                  }
              }
              doActions(local);
              return true;
          }
          return false;
      }

      bool isParameterModifier(string parameterType) {
          return parameterType == "ref" || parameterType == "out";
      }
  }

 /////////////////////////////////////////////////////////////////
 // Detects user defined struct or enum usage as composition
  public class DetectComposition : ARule {
      public override bool test(CSsemi.CSemiExp semi)
      {
          // Returns true when a struct or enum usage is found inside a class
          if (semi[semi.count - 1] != ";")
              return false;
          int firstNonNewLineToken = GetFirstNonNewLineIndex(semi);
          int compostionTypeCandidate = firstNonNewLineToken;
          if (isAccessModifier(semi[firstNonNewLineToken])) {
              compostionTypeCandidate++;
          }
          if (semi.count - compostionTypeCandidate == 3)
          {
              if (!isKeyWord(semi[compostionTypeCandidate]))
              {
                  CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                  local.Add("Composition").Add(semi[compostionTypeCandidate]);
                  doActions(local);
                  return true;
              }
         
          }
          return false; 
      }

      private int GetFirstNonNewLineIndex(CSsemi.CSemiExp semi)
      {
          int i = 0;
          for (; i < semi.count; i++)
          {
              if (semi[i] != "\n")
                  break;
          }
          return i;
      }

      bool isAccessModifier(string token)
      { 
        string[] tokens = { "private", "public"};
        foreach (string stoken in tokens)
           if (stoken == token)
                  return true;
          return false; 
      }
      bool isKeyWord(string word) {
          string[] keyword = { "return", "int", "double", "char", "string", "using" };
          foreach (string stoken in keyword)
              if (stoken == word)
                  return true;
          return false; 
      }
   
   }
  
    
  public class BuildCodeAnalyzer
  {
    Repository repo = new Repository();

    public BuildCodeAnalyzer(CSsemi.CSemiExp semi)
    {
      repo.semi = semi;
    }

    // Build a Parser to retrieve Function Complexities or user defined Types
    public virtual Parser build()
    {
      Parser parser = new Parser();
         
      // action used for namespaces, classes, and functions
      PushStack push = new PushStack(repo);
      AddUserDefinedType addUserDefinedType = new AddUserDefinedType(repo);

      // capture namespace info
      DetectNamespace detectNS = new DetectNamespace();
      detectNS.add(push);
      parser.add(detectNS);

      // capture class info
      DetectType detectCl = new DetectType();
      detectCl.add(push);
      detectCl.add(addUserDefinedType);
      parser.add(detectCl);
     

      // capture function info
      DetectFunction detectFN = new DetectFunction();
      detectFN.add(push);
      parser.add(detectFN);

      // handle entering anonymous scopes, e.g., if, while, etc.
      DetectAnonymousScope anon = new DetectAnonymousScope();
      anon.add(push);
      parser.add(anon);


      // handle leaving scopes
      DetectLeavingScope leave = new DetectLeavingScope();
      PopStack pop = new PopStack(repo);

      leave.add(pop);
      parser.add(leave);

      // handle entering braceless Scopes
      DetectBracelessScope braless = new DetectBracelessScope();
      parser.add(braless);

      // parser configured
      return parser;
    }


    // Build parser to detect Relationships among user defined types
    public virtual Parser buildParserForRelationships(TypeTable interestedTypes,TypeTable allTypes)
    {
        Parser parser = new Parser();

        PushStack push;
        AddRelationship addRelationship;

        AddActions(interestedTypes,allTypes, out push, out addRelationship);
        AddRules(parser, push, addRelationship);

        // parser configured
        return parser;
    }

    private void AddRules(Parser parser, PushStack push, AddRelationship addRelationship)
    {
        // capture namespace info
        DetectNamespace detectNS = new DetectNamespace();
        detectNS.add(push);
        parser.add(detectNS);

        // capture Aggregation
        DetectAggregation detectAg = new DetectAggregation();
        detectAg.add(addRelationship);
        parser.add(detectAg);

        // capture class info
        DetectInheritance detectIn = new DetectInheritance();
        detectIn.add(push);
        detectIn.add(addRelationship);
        parser.add(detectIn);


        // capture function info and try to detect using relationship
        DetectUsing detectUs = new DetectUsing();
        detectUs.add(push);
        detectUs.add(addRelationship);
        parser.add(detectUs);

        // handle entering anonymous scopes, e.g., if, while, etc.
        DetectAnonymousScope anon = new DetectAnonymousScope();
        anon.add(push);
        parser.add(anon);

        // handle leaving scopes
        DetectLeavingScope leave = new DetectLeavingScope();
        PopStack pop = new PopStack(repo);
        leave.add(pop);
        parser.add(leave);


        // capture Composition     
        DetectComposition detectComp = new DetectComposition();
        detectComp.add(addRelationship);
        parser.add(detectComp);
    }

    private void AddActions(TypeTable interestedTypes, TypeTable allTypes, out PushStack push, out AddRelationship addRelationship)
    {
        // action used for namespaces, classes, and functions
        push = new PushStack(repo);
        addRelationship = new AddRelationship(repo, interestedTypes, allTypes);
    }

    
  }
}

