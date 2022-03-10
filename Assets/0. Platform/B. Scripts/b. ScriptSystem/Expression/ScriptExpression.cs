using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PIERStory
{
    [System.Serializable]

    public class ScriptExpression
    {
        public int prioriry = 0; // 우선순위
        public string expression = string.Empty; // 조건문 원문.
        public bool isOperator = false;

        public int ID = -1;
        
        [SerializeField] string origin = string.Empty;
        [SerializeField] string speaker = string.Empty; // 화자 
        [SerializeField] string ability = string.Empty; // 능력이름 
        [SerializeField] int conditionValue = 0; // 조건 값 
        [SerializeField] int currentValue = 0;
        
        [SerializeField] string inequation = string.Empty; // 부등식 수식 
        
        const string prefixAbility = "@";
        
        
        

        /// <summary>
        /// 새 표현식 생성
        /// </summary>
        /// <param name="__exp"></param>
        /// <param name="__priority"></param>
        public ScriptExpression(string __exp, int __priority, bool __operator, int __id)
        {
            origin = __exp;
            expression = __exp;
            prioriry = __priority;
            isOperator = __operator;

            ID = __id;
        }

        /// <summary>
        /// 표현식의 참 유무 체크 
        /// </summary>
        /// <returns></returns>
        public bool CalcExpression()
        {

            if (expression == "true" || expression == "True")
                return true;
            else if (expression == "false" || expression == "False")
                return false;


            // [] 여부 체크 한다. 그룹 체크 
            if (expression.Contains("[") && expression.Contains("]"))
            {
                Debug.Log(string.Format("Square Expression {0} ", expression));

                // 그룹의 경우 validation check를 한다. 
                // :과 , 체크 
                if(!expression.Contains(",") || !expression.Contains(":"))
                {
                    Debug.Log(string.Format("!!! Missing Group expression component !!!"));
                    return true;
                }

                // 기존 expression에서 대괄호를 제거 
                string exp = expression.Replace("[", "").Replace("]", "");
                string condition = string.Empty;
                int conditionCount = 0;
                bool parseChecker = false;

                Debug.Log(string.Format("Square #0 exp : {0}", exp));

                condition = exp.Split(':')[1]; // 먼저 수식을 받고 
                exp = exp.Split(':')[0]; // 수식의 원문과 조건으로 분기 
                

                parseChecker = int.TryParse(condition, out conditionCount);

                // 조건이 숫자가 아닌 경우 오류 
                if(!parseChecker)
                {
                    Debug.Log(string.Format("!!! Group Condition Wrong {0} !!!", condition));
                    return true;
                }

                // 여기까지 왔으면 "101,102,103" 형식의 exp와 숫자형태의 conditionCount가 남는다. 
                string[] expSet = exp.Split(',');
                int countingTrue = 0;

                Debug.Log(string.Format("Square #1 exp: {0} / , cond: {1}", exp, conditionCount));

                for(int i=0; i<expSet.Length;i++)
                {
                    if (CalcSingleExpression(expSet[i]))
                        countingTrue++;
                }

                Debug.Log(string.Format("Square #2 countingTrue: {0}, / conditionCount: {1}", countingTrue, conditionCount));


                if (countingTrue >= conditionCount)
                    return true;

                return false;
            }
            else // 없을 때.. 
            {
                Debug.Log(string.Format("single Expression {0} ", expression));

                return CalcSingleExpression(expression);
            }


        }


        
        /// <summary>
        /// 단독 상황 ID 체크 
        /// </summary>
        /// <param name="__expression"></param>
        /// <returns></returns>
        bool CalcSingleExpression(string __expression)
        {
            bool isExclamation = false;
            string exp = __expression;
            bool isSceneID = false;
            int scene_id = 0;
            
            Debug.Log(string.Format("### CalcSingleExpression Start[{0}]", exp));
          
            
            // * 2022.03 
            // * 능력치 조건 추가 
            if(exp.Contains(prefixAbility)) {

                Debug.Log("### It's ability expression");

                string[] spliteExp;
                
                // 필수적으로 '_' 화자,능력치 구분자
                // >, <, =, >=, <= 5개의 부등식 필요하다. 
                
                if(!exp.Contains("_")) {
                    Debug.Log(string.Format("!!! Can't Verify this {0} !!!", expression));
                    return true; // 일단 true로 준다.
                }
                
                // 부등식 수식 체크...
                if(exp.Contains("<=")) {
                    inequation = "<=";
                }
                else if(exp.Contains(">=")) {
                    inequation = ">=";
                }
                else if(!exp.Contains("=") && exp.Contains("<")) {
                    inequation = "<";
                }
                else if(!exp.Contains("=") && exp.Contains(">")) {
                    inequation = ">";
                }
                else if(!exp.Contains("<") && !exp.Contains(">") && exp.Contains("=")) {
                    inequation = "=";
                }
                else {
                    Debug.Log(string.Format("!!! Can't Verify this {0} !!!", expression));
                    return true; // 일단 true로 준다.
                } // ? END 부등식 체크 종료
                
                exp = exp.Replace(prefixAbility, ""); // 골뱅이제거
                spliteExp = exp.Split('_'); // 화자 추출 
                speaker = spliteExp[0]; 
                
                // 화자 체크 
                if(string.IsNullOrEmpty(speaker)) {
                    Debug.Log(string.Format("!!! Wrong speaker {0} !!!", expression));
                    return true; 
                }
                
                exp = spliteExp[1]; // @케일_매력>=80 에서 매력>=80 만 남기기. 
                ability = exp.Split(inequation[0])[0]; // 능력 빼내기. 
                
                // 능력이름 체크
                if(string.IsNullOrEmpty(ability)) {
                    Debug.Log(string.Format("!!! Wrong ability {0} !!!", expression));
                    return true; 
                }
                
               // 값 빼내기 
                exp = exp.Replace(ability, "") ; // >=80 만 남기고
                exp = exp.Replace(inequation, ""); // 80만 남긴다. 
                
                // 조건 값 파싱 
                int.TryParse(exp, out conditionValue);
                
                // 조건 값 체크 
                if(conditionValue == 0) {
                    // 0이면.. 의심해봐야한다. (리턴은 하지 않음)
                    Debug.Log("#### the ability value is zero " + expression);
                }
                
                // 화자, 능력, 조건값을 찾았으면 UserManager 통해서 체크할것
                currentValue = UserManager.main.GetSpeakerAbilityValue(speaker, ability);
                
                Debug.Log(string.Format("Speaker[{0}]/Ability[{1}]/current[{2}]/condition[{3}]", speaker, ability, currentValue, conditionValue));
                
                // 이제 수식에 따라.. 체크 ㅠ
                switch(inequation) {
                    case "<":
                        return conditionValue > currentValue; // 현재 값이 조건보다 작다
                    case ">":
                        return conditionValue < currentValue; // 현재 값이 조건보다 크다
                    case "=":
                        return conditionValue == currentValue; // 같다. 
                    case ">=":
                        return conditionValue <= currentValue; // 현재 값이 조건보다 크거나 같다. 
                    case "<=":
                        return conditionValue >= currentValue; // 현재 값이 조건보다 작거나 같다. 
                }
                
                // 여기까지 오면 안되는데..?
                Debug.Log("#### weird ability inequation " + expression);
                return true;
                
            } // ? END 능력치 조건 처리 완료
            
            

            if (exp.Contains("!"))
            {
                isExclamation = true;
                exp = exp.Replace("!", ""); // 느낌표를 제거해준다. 
            }

            isSceneID = int.TryParse(exp, out scene_id);

            // 상황 ID가 포맷이 맞을때!
            if(isSceneID)
            {
                if (isExclamation)
                    return !UserManager.main.CheckSceneProgress(exp);
                else
                    return UserManager.main.CheckSceneProgress(exp);
            }
            else
            {
                Debug.Log(string.Format("!!! Can't Verify this {0} !!!", expression));
                return true; // 일단 true로 준다.
            } 

        }
    }
}