using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace PIERStory
{



    /// <summary>
    /// 스크립트 Parser.
    /// </summary>
    public class ScriptExpressionParser : MonoBehaviour
    {

        // 사용하는 괄호는 딱 4가지. 
        public static readonly string Lpa = "("; 
        public static readonly string Rpa = ")";
        public static readonly string Lsb = "[";
        public static readonly string Rsb = "]";

        const char cLpa = '(';
        const char cRpa = ')';
        const char cLsb = '[';
        const char cRsb = ']';

        const char binaryAnd = '&';
        const char binaryOr = '|';


        public string expression = string.Empty;
        public List<ScriptExpression> ListExpression = new List<ScriptExpression>(); // 표현식 모음
        public List<ScriptExpression> ListMergedExpression = new List<ScriptExpression>(); // 표현식 통합, 복사 용도의 리스트 

        List<ScriptExpression> ListCollectedExpression = new List<ScriptExpression>(); //우선순위끼리 모여진 표현식 모음 

        public int expressionID = 0;
        public bool finalExpressionResult = false;

        public static ScriptExpressionParser main = null;

        private void Awake()
        {
            main = this;
        }

        /// <summary>
        /// 조건문 파싱하기 
        /// </summary>
        /// <param name="__expression"></param>
        public bool ParseScriptExpression(string __expression)
        {

            expression = __expression.Replace(" ", ""); // 공백제거 
            expressionID = 0;

            int maxPriority = 0; // 우선순위 체크용 
            bool isBeginCollecting = false; // 

            ListMergedExpression.Clear();
            

            // 유효성 체크하면서 모은다. 
            if (!CheckExpressionValidation())
            {
                Debug.Log("Wrong Expression");
                return false;
            }

            

            maxPriority = GetMaxPrioriry();
            Debug.Log(string.Format("First Max Prioirity {0}", maxPriority));


            // maxPriority가 0보다 클때 계속 반복 
            while (maxPriority >= 0 )
            {
                Debug.Log(string.Format("maxPriority is collecting : {0}", maxPriority));

                isBeginCollecting = false;
                ListCollectedExpression.Clear();
                ListMergedExpression.Clear();

                // 수집 시작!
                for (int i=0; i<ListExpression.Count;i++)
                {
                    if(ListExpression[i].prioriry == maxPriority)
                    {
                        if (!isBeginCollecting)
                            isBeginCollecting = true;

                        // 수집을 시작한다. 
                        ListCollectedExpression.Add(ListExpression[i]);
                    }
                    else // 다른 우선순위를 만남!(인접한 것들만 수집)
                    {
                        // 수집이 되다가 다른 priority를 만나게되면 그 즉시 수집을 중단하고 while을 빠져나간다. 
                        if(isBeginCollecting)
                        {
                            break;
                        }
                    }


                } // end of i for

                Debug.Log(string.Format("maxPriority collection count : {0}", ListCollectedExpression.Count));



                // for문 나와서 수집된거 가지고 처리시작
                // 수집된게 없으면 이상한.... 
                if (ListCollectedExpression.Count > 0)
                {
                    ScriptExpression mergedExp = MergeExpressions(ListCollectedExpression); // 합체! 


                    foreach (ScriptExpression exp in ListExpression)
                    {
                        // 수집되지 않은것만 넣는다. 
                        if (!CheckExistsExpressionInList(exp, ListCollectedExpression))
                            ListMergedExpression.Add(exp);
                    }

                    // 다 넣고 mergedExp 추가
                    ListMergedExpression.Add(mergedExp);

                    // 여기서 ListExpression으로 정렬해서 넣어준다. (id로 정렬하는데..)
                    ListExpression = ListMergedExpression.OrderBy(x => x.ID).ToList<ScriptExpression>();


                    // 수집된거 하나도 없으면 maxPriority 감소, 우선순위가 0보다 큰 같은 인접한! 수식이 여러개 있을수도 있다. 
                    /*
                    if (!isBeginCollecting)
                        maxPriority--;
                    */
                }
                else
                {
                    // Debug.LogError("<color=yellow>No Collected Expression!!!!!! </color>");
                    // 하나도 없을때만 감소시킨다.
                    maxPriority--;
                    // break;
                }


            } // end of while


            if(ListExpression.Count == 0 || ListExpression.Count > 1)
            {
                Debug.Log(string.Format("!!! Expression Parse Error!! final expression count : {0}. ", ListExpression.Count));
                finalExpressionResult = false;
                return false;
            }


            finalExpressionResult = ListExpression[0].CalcExpression();

            return finalExpressionResult;



        }


        /// <summary>
        /// 특정 리스트에 해당 표현문이 있는지 유무 체크 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        bool CheckExistsExpressionInList(ScriptExpression exp, List<ScriptExpression> list)
        {
            for(int i=0; i<list.Count; i++)
            {
                if (exp.ID == list[i].ID)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// 주어진 조건문을 계산해서 통합 
        /// </summary>
        /// <param name="__list"></param>
        /// <returns></returns>
        ScriptExpression MergeExpressions(List<ScriptExpression> __list)
        {
            bool hasBinaryOperator = false; // 바이너리 오퍼레이터 보유 여부 ( &, | ) 
            bool hasOrOperator = false;
            bool finalResult = false; // 최종 결과
            int mergedID = __list[0].ID; // 하나로 통합되었을때 부여되는 ID 
            int priority = __list[0].prioriry;
            

            foreach (ScriptExpression exp in __list)
            {
                if (exp.isOperator)
                    hasBinaryOperator = true;

                if (exp.isOperator && exp.expression == binaryOr.ToString())
                    hasOrOperator = true; // or 연산자 있음!

            }

            // binary Operator 있을때 처리 
            if(hasBinaryOperator)
            {
                if(hasOrOperator) // or 연산자가 있을때는 표현식에 true가 하나라도 있으면 이건 true야. 
                {
                    foreach(ScriptExpression exp in __list)
                    {
                        if(!exp.isOperator && exp.CalcExpression())
                        {
                            finalResult = true;
                            break;
                        }
                    }
                } 
                else // or연산자가 하나도 없을때!
                {
                    finalResult = true; // true라고 해놓고 하나라도 false가 나오면 false 처리 

                    foreach (ScriptExpression exp in __list)
                    {
                        // 하나라도 false가 있으면 false!
                        if (!exp.isOperator && !exp.CalcExpression())
                        {
                            finalResult = false;
                            break;
                        }
                    }
                }
            } // binaryOperator 처리 끝!
            else // binaryOperator가 없을때. 
            {

                if (__list.Count > 1)
                {
                    Debug.LogError("연산자가 없는데 표현식이 여러개라니!! ");
                    return null;
                }

                finalResult = __list[0].CalcExpression();
            }

            priority--; // 1감소 시키기. 

            return new ScriptExpression(finalResult.ToString(), priority, false, mergedID);
        } // end of MergeExpressions;


        /// <summary>
        /// 최대 우선순위를 구한다. 
        /// </summary>
        /// <returns></returns>
        int GetMaxPrioriry()
        {
            int maxPriority = 0;

            for(int i=0; i<ListExpression.Count;i++)
            {
                if (ListExpression[i].prioriry > maxPriority)
                    maxPriority = ListExpression[i].prioriry;
            }


            return maxPriority;
        }


        /// <summary>
        /// 유효성 체크 
        /// </summary>
        /// <returns></returns>
        public bool CheckExpressionValidation()
        {

            ListExpression.Clear();

            // 괄호 체크를 진행한다. 
            if (WordCheck(expression, Lpa) != WordCheck(expression, Rpa))
            {
                Debug.Log("Not match parentheses");
                return false;
            }

            // 대괄호 체크 
            if (WordCheck(expression, Lsb) != WordCheck(expression, Rsb))
            {
                Debug.Log("Not match SB");
                return false;
            }

            // while 문으로 돌려보자
            int index = 0;
            int priority = 0;
            char c;
            bool isSquareOpen = false; // '[' 시작되었는지 체크 
            string singleExpression = string.Empty; // 쪼개진 조건식

            

            
            while(index < expression.Length)
            {
                // 한글자씩 체크하면서 오퍼레이터 만나면 string으로 모아서 전달
                c = expression[index]; // 한글자씩!

                // 공백을 만나면 continue 시켜주자. 
                if (char.IsWhiteSpace(c))
                {
                    index++;
                    continue;
                }

                switch(c)
                {
                    // 자.. binary operator를 만나면 그 즉시 종료된다. 
                    case binaryAnd:
                    case binaryOr:
                        // 이제까지 만들어진 singleExpression으로 ScriptExpression을 생성해서 리스트에 추가 
                        if (!string.IsNullOrEmpty(singleExpression))
                        {
                            AddExpresion(ref singleExpression, priority, false, expressionID++);
                        }

                        // 현재 binary operator를 추가 
                        ListExpression.Add(new ScriptExpression(c.ToString(), priority, true, expressionID++));
                        index++; // 인덱스 증가시켜주고 continue;
                        continue;

                    case cLpa: // '('
                        priority++; // 우선순위 1증가 시켜준다. 
                        index++;
                        continue;
                        

                    case cRpa: // ')' 


                        // 괄호 닫힐때, 작성된 수식이 있으면/                        
                        if(!string.IsNullOrEmpty(singleExpression))
                        {
                            AddExpresion(ref singleExpression, priority, false, expressionID++);
                            
                        }

                        priority--; // 1감소  
                        index++;
                        continue;
                        
                } // 만나면 종료 처리되는 친구들. 



                singleExpression += c; // 한글자씩 더해준다. 
                // '[' 만나면 isSquareOpen을 true로 변경한다. 
                if (c == cLsb)
                    isSquareOpen = true;

                if( c== cRsb )
                {
                    if (!isSquareOpen)
                    {
                        Debug.Log("Wrong Square Close");
                        return false;
                    }
                    else
                    {
                        isSquareOpen = false; // 닫아주기 
                    }
                }

                index++;

            } // end of while... 


            // 나왔을때 singleExpression 점검해서 추가 
            
            if(!string.IsNullOrEmpty(singleExpression))
                AddExpresion(ref singleExpression, priority, false, expressionID++);
            

            return true;
        }


        void AddExpresion(ref string __exp, int __priority, bool __operator, int __id)
        {
            ListExpression.Add(new ScriptExpression(__exp, __priority, __operator, __id));
            __exp = string.Empty;
        }


        /// <summary>
        /// 단어 체크!
        /// </summary>
        /// <param name="__origin"></param>
        /// <param name="__word"></param>
        /// <returns></returns>
        int WordCheck(string __origin, string __word)
        {
            string[] StringArray = __origin.Split(new string[] { __word }, StringSplitOptions.None);

            return StringArray.Length - 1;
        }


        /// <summary>
        /// 표현식 쪼개기.
        /// </summary>
        /// <param name="__originalExpression"></param>
        /// <returns></returns>
        public List<ScriptExpression> SplitExpression(string __originalExpression)
        {
            return null;
        }

    }
}