using System;

namespace PIERStory {

    public interface IRowAction
    {


        /// <summary>
        /// 액션 수행 
        /// </summary>
        /// <param name="__actionCallback"></param>
        void DoAction(Action __actionCallback, bool __isInstant = false);

        // void DoAction(Action __actionCallback, bool __isInstant);


        void EndAction();
    }
}
