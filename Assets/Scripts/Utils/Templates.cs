using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Debug;

namespace Templates {
	namespace FSM {
		public delegate bool FSMTransition(ref object enter, ref object exit);

		public class FSMState<T> {
			public T self;
			public FSM<T> fsm;
			private readonly List<(FSMTransition, Type)> transitions = new();
			public virtual void Enter(object param) { }
			public virtual void Exit(object param) { }
			public virtual void Update() { }
			public void AddTransition<S>(FSMTransition cond) {
				transitions.Add((cond, typeof(S)));
			}
			public void Translate() {
				foreach (var (cond, to) in transitions) {
					object enter = null, exit = null;
					if (!cond(ref enter, ref exit)) continue;
					fsm.Translate(to, enter, exit);
					return;
				}
			}
		}
		public class FSM<T> {
			private readonly T self;
			private readonly Dictionary<Type, FSMState<T>> stateDic;
			public FSMState<T> currentState { get; private set; }
			public FSMState<T> previousState { get; private set; }
			public bool isRunning { get { return currentState != null; } }
			public FSM(T self) {
				this.self = self;
				stateDic = new Dictionary<Type, FSMState<T>>();
				previousState = currentState = null;
			}
			public void AddState(params FSMState<T>[] states) {
				foreach (var state in states) {
					Assert(!stateDic.ContainsKey(state.GetType()), "State already exists.");
					state.self = self; state.fsm = this;
					stateDic.Add(state.GetType(), state);
				}
			}
			public void AddState<S>() where S : FSMState<T>, new() {
				Assert(!stateDic.ContainsKey(typeof(S)), "State already exists.");
				stateDic.Add(typeof(S), new S() { self = self, fsm = this });
			}

			public void Translate(Type state, object enter = null, object exit = null) {
				Assert(stateDic.ContainsKey(state), $"State {state.Name} not exists.");
				ExitState(exit);
				currentState = stateDic[state];
				currentState.Enter(enter);
			}
			public void Translate<S>(object enter = null, object exit = null) {
				Translate(typeof(S), enter, exit);
			}
			public void ExitState(object exit = null) {
				var backup = currentState;
				currentState = null;
				if (backup != null) backup.Exit(exit);
				previousState = backup;
			}
			public void Update() {
				if (currentState == null) return;
				currentState.Update();
				currentState.Translate();
			}
		}
	}
	namespace Singleton {
		public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
			public static T instance { get; private set; }
			protected virtual void Awake() {
				if (instance == null) {
					instance = this as T;
				} else Destroy(gameObject);
			}
		}
		public class Singleton<T> where T : Singleton<T>, new() {
			private static T m_instance = null;
			public static T instance => m_instance ??= new T();
		}
	}
}