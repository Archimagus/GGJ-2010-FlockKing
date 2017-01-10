using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;

//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace FlocKing
{
    public class CStateMachine
    {
        private List<IBaseState> m_lStates = new List<IBaseState>();

        static readonly CStateMachine instance = new CStateMachine();

       public static CStateMachine Instance { get { return instance; } }

        public CStateMachine()
        {
        }

        ~CStateMachine()
        {
            while (m_lStates.Count != 0)
            {
                m_lStates[0].Exit();
                m_lStates.RemoveAt(0);
            }
        }
        public void PushState(IBaseState _State)
        {
            if (_State != null)
            {
                m_lStates.Add(_State);
                m_lStates[m_lStates.Count - 1].Enter();
            }

        }
        public void PopState(IBaseState _State)
        {
            if (m_lStates.Count > 1)
            {
                m_lStates[m_lStates.Count - 1].Exit();
                m_lStates.RemoveAt(m_lStates.Count - 1);
            }
        }
        public void ChangeState(IBaseState _State)
        {
            while (m_lStates.Count != 0)
            {
                m_lStates[0].Exit();
                m_lStates.RemoveAt(0);
            }
            if (_State != null)
                m_lStates.Add(_State);
            if (m_lStates.Count != 0)
                m_lStates[m_lStates.Count - 1].Enter();

        }
        public void Render()
        {
            if(m_lStates.Count != 0)
            for (int i = 0; i < m_lStates.Count; ++i)
                           m_lStates[i].Render();
            
        }
        public void Update(GameTime _delta)
        {
            if(m_lStates.Count != 0)
                m_lStates[m_lStates.Count - 1].Update(_delta);

        }
        public bool Input()
        {
            if (m_lStates.Count != 0)
            {
                if (!m_lStates[m_lStates.Count - 1].Input())
                    return false;
            }

                    return true;

        }

    }
}
