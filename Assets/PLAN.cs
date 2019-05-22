using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System.IO;


using UnityEngine.Events;

//using System.Linq;

//using UnityEngine;


namespace PLAN {


    public struct PTime {

        public const decimal DistantPast_utc = -5.5e+28m;
        public const decimal DistantFuture_utc = 5.5e+28m;

        public static readonly PTime DistantPast   = new PTime( DistantPast_utc );
        public static readonly PTime DistantFuture = new PTime( DistantFuture_utc );

        public static
        PTime Now() {
            TimeSpan span = DateTime.UtcNow.Subtract( utc1970 );
            return new PTime( (decimal) span.TotalSeconds );
        }

        public static
        DateTime utc1970 = new DateTime( 1970, 1, 1 );


        public PTime( PTime inTimestamp ) {
            utc = inTimestamp.utc;
        }

        public PTime( decimal inUTC ) {
            utc = inUTC;
        }

        // Number of seconds elapsed since Jan 1, 1970 UTC
        public
        decimal utc;

        public
        bool Equals( PTime inTime ) { return utc == inTime.utc; }

        public
        bool IsLaterThan( PTime inTime ) {
            return utc > inTime.utc;
        }

        public
        bool IsEarlierThan( PTime inTime ) {
            return utc < inTime.utc;
        }

    }


    public
    class PActionQueue {

        // THREADSAFE
        // Synchronously executes all actions (on the current thead) that have been enqueued since the last call to ExecuteActions()
        public
        void ExecuteActions() {

            {

                if ( _todoQueue.Count > 0 ) {

                    // Swap active queues so we don't have to worry about locking
                    lock ( _todoQueue ) {
                        List<Action> off = _todoQueue;
                        _todoQueue = _offQueue;
                        _offQueue = off;
                    }

                    int N = _offQueue.Count;
                    for ( int i = 0; i < N; i++ ) {
                        _offQueue[i]();
                    }

                    _offQueue.Clear();

                }
            }
        }

        // THREADSAFE
        // Enqueues the given action for execution on the next call to ExecuteActions()
        public
        void EnqueueAction( Action inAction ) {

            lock ( _todoQueue ) {
                _todoQueue.Add( inAction );
            }

        }

        private List<Action> _offQueue  = new List<Action>( 55 );
        private List<Action> _todoQueue = new List<Action>( 55 );
    }


    public
    interface IAnonymousInit {

        void OnAnonymousInit();

    }

    //
    // The point of scriptable objects is that multiple objects can reference one scriptable object instance AND those references will be correctly 
    //    serialized (if you do that with plain classes, you'll get duplicates on deseralization). The references can be assigned by drag and drop. 
    // 
    // Moneyshot on generics: https://msdn.microsoft.com/en-us/library/ms379564(v=vs.80).aspx
    public abstract
    class SingletonObject<T> : ScriptableObject where T : ScriptableObject, IAnonymousInit {

        public static T Instance {
            get {
                if ( ! _instance ) {

                    // See time 23:00 at https://unity3d.com/learn/tutorials/topics/scripting/overthrowing-monobehaviour-tyranny-glorious-scriptableobject
                    // Note that FindObjectsOfTypeAll() only return already existing assets -- created via Assets menu or created and saved via scripting;
                    var insts = Resources.FindObjectsOfTypeAll<T>();
                    foreach ( T i in insts ) {
                        _instance = i;
                        break;
                    }
                    if ( _instance == null ) {
                        _instance = CreateInstance<T>();
                        _instance.OnAnonymousInit();
                    }
                }
                return _instance;
            }
        }

        private static T _instance = null;

    }

    public abstract
    class RuntimeSet<T> : ScriptableObject {

        public List<T> Items = new List<T>();

        public void Add( T inItem ) {
            if ( !Items.Contains( inItem ) ) {
                Items.Add( inItem );
            }
        }

        public void Remove( T inItem ) {
            if ( !Items.Contains( inItem ) ) {
                Items.Remove( inItem );
            }
        }
    }

    [CreateAssetMenu( menuName = "PLAN/PNumber" )]
    public class PNumber : ScriptableObject {

#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif

        public decimal Value;

        public
        void SetValue( decimal inVal ) {
            Value = inVal;
        }

        public
        void SetValue( PNumber inVal ) {
            Value = inVal.Value;
        }

        public
        void ApplyDelta( decimal inDelta ) {
            Value += inDelta;
        }

        public
        void ApplyChange( PNumber inAmt ) {
            Value += inAmt.Value;
        }
    }


    // Use this: https://gist.github.com/nickgravelyn/2480060
    //
    /*
	[CreateAssetMenu(menuName = "PLAN/PEvent")]
	public class PEvent : ScriptableObject {

		private readonly List<PEventListener<T>> eventListeners = new List<PEventListener<T>>();

		public void Raise() {

			// Traverse backwards to help if listeners remove themselves after OnEventRaised()
			for ( int i = eventListeners.Count - 1; i >= 0; i-- ) {
				eventListeners[i].OnEventRaised();
			}
		}

		public void RegisterListener( PEventListener<T> inListener ) {

			if ( ! eventListeners.Contains( inListener ) )
				eventListeners.Add( inListener );
		}

		public void UnregisterListener( PEventListener<T> inListener ) {

			if ( eventListeners.Contains( inListener ) )
				eventListeners.Remove( inListener );
		}
	}*/

    /*
	public
	class PBroadcaster<T> {


		public 
		void Raise() {

			// Traverse backwards to help if listeners remove themselves after OnEventRaised()
			for ( int i = eventListeners.Count - 1; i >= 0; i-- ) {
				_listeners.OnEventRaised();
			}
		}

		public 
		void RegisterListener( PListener<T> inListener ) {

			if ( ! _listeners.Contains( inListener ) )
				_listeners.Add( inListener );
		}

		public 
		void UnregisterListener( PListener<T> inListener ) {

			if ( _listeners.Contains( inListener ) )
				_listeners.Remove( inListener );
		}



		private readonly List<PListener<T>> _listeners = new List<PListener<T>>();

	}

/*
	public class PListener<T> : MonoBehaviour {

		public
		PBroadcaster<T>				PB

		void OnEnable() {
			PEvent.RegisterListener( this );
		}


		void OnDisable() {
			PEvent.UnregisterListener( this );
		}


		public 
		void OnEventRaised( T inOriginator ) {
			Response.Invoke();
		}
	}



	//class SingletonObject<T> : ScriptableObject where T : ScriptableObject {  
		

	public 
	class PEventClient<T> : MonoBehaviour {

		[Tooltip("Event to register with.")]
		public PEvent PEvent;

		[Tooltip("Response to invoke when Event is raised.")]
		public UnityEvent Response;

		public
		T						


		void OnEnable() {
			PEvent.RegisterListener( this );
		}


		void OnDisable() {
			PEvent.UnregisterListener( this );
		}


		public 
		void OnEventRaised() {
			Response.Invoke();
		}
	}


	public class PListener<T> : MonoBehaviour {

		[Tooltip("Event to register with.")]
		public SingletonObject<T> PBroadcaster;

		[Tooltip("Response to invoke when Event is raised.")]
		public UnityEvent Response;


		void OnEnable() {
			PBroadcaster.RegisterListener( this );
		}


		void OnDisable() {
			PBroadcaster.UnregisterListener( this );
		}


		public 
		void OnEventRaised( T inOriginator ) {
			Response.Invoke();
		}
	}  */






    public
    enum PErrCode : uint {
        noErr = 0,

        ChannelIsClosing = 4000,

        DataHostInvocationNotFound = 5001,
        DataHostAlreadyMounting = 5002,
    };

    public
    class PErr {

        public PErr( PErrCode inCode ) {
            Code = inCode;

        }
        public PErrCode Code = PErrCode.noErr;


        public static
        void assert( bool inCond ) {
            if ( !inCond ) {
                Debug.Assert( false );
            }
        }

    }


    public delegate void PAction( object inParam, PErr inErr );



    public class PActionRegistry {

        // [] operator
        public PAction this[string inActionKey] {
            get {
                PAction val = null;
                _actionsByStr.TryGetValue( inActionKey, out val );
                return val;
            }
            set {
                if ( inActionKey != null ) {
                    if ( value == null ) {
                        _actionsByStr.Remove( inActionKey );
                    } else {
                        _actionsByStr[inActionKey] = value;
                    }
                }
            }
        }


        public bool HasAction( string inActionKey ) {
            return _actionsByStr.ContainsKey( inActionKey );
        }


        public bool StepInto( string inActionKey, object inParam, PErr inErr ) {
            PAction action = null;
            _actionsByStr.TryGetValue( inActionKey, out action );
            if ( action != null ) {
                action( inParam, inErr );
                return true;
            } else {
                return false;
            }
        }


        private Dictionary<string, PAction> _actionsByStr = new Dictionary<string, PAction>();

    }



}




namespace PLAN {


    [Serializable]
    public
    class PSettingsStore {

        public
        bool GetBool( string inKey ) {
            bool value = false;
            if ( inKey != null )
                value = Convert.ToBoolean( _dict[inKey] );

            return value;
        }

        public
        long GetInt( string inKey ) {
            long value = 0;
            if ( inKey != null )
                value = Convert.ToInt64( _dict[inKey] );

            return value;
        }

        public
        double GetDouble( string inKey ) {
            double value = 0;
            if ( inKey != null )
                value = Convert.ToDouble( _dict[inKey] );

            return value;
        }

        public
        string GetStr( string inKey ) {
            string value = null;
            if ( inKey != null )
                value = Convert.ToString( _dict[inKey] );

            return value;
        }

        public
        object Get( string inKey ) {
            object value = null;
            if ( inKey != null )
                _dict.TryGetValue( inKey, out value );

            return value;
        }


        public
        void Set( string inKey, object inValue ) {
            if ( inKey != null ) {
                _dict[inKey] = inValue;
            }
        }

        public
        void PreSave() {

        }

        public
        void PostLoad() {

        }

 


        Dictionary<string, object> _dict = new Dictionary<string, object>();


    }


    public
    class PSettings {
        
        public PSettingsStore store { get; private set; }

        public
        void MapToFile( string inRelativePathname ) {

            _mappedFilename = string.Format( "{0}/{1}", Application.persistentDataPath, inRelativePathname );

        }

        public 
        bool LoadFromStorage() {

            if ( ! File.Exists( _mappedFilename ) ) {
                store = new PSettingsStore();
                return false;
            }

            //return new StreamWriter( new FileStream( m_Filename, FileMode.Create ) );
        

            var reader = new StreamReader( new FileStream( _mappedFilename, FileMode.Open ) );
            using ( reader ) {
                store = JsonUtility.FromJson<PSettingsStore>( reader.ReadToEnd() );
            }

            return true;
        }


        public
        bool UpdateStorage() {

            string json = JsonUtility.ToJson( store );

            var writer = new StreamWriter( new FileStream( _mappedFilename, FileMode.Create ) );

            using ( writer ) {
                writer.Write( json );
            }


            return true;
        }


        string _mappedFilename;

    }

       
    // Instantiated via edit (e.g. "PRuntime-main")
    [CreateAssetMenu( menuName = "PLAN/PRuntime" )]
    public
    class PRuntime : SingletonObject<PRuntime>, IAnonymousInit {


        public
        void InitiateShutdown( Action inOnCompletion = null ) {

            _pendingShutdown = inOnCompletion;

        }

        // Set when InitiateShutdown() is called 
        protected
        Action _pendingShutdown;



        public
        IEnumerator DoPeriodicals() {

            for ( ; ; ) {

                _pendingActions.ExecuteActions();

                yield return _periodicalsDelay;
            }
        }



        public
        void EnqueueAction( Action inAction ) {

            if ( inAction != null ) {
                _pendingActions.EnqueueAction( inAction );
            }
        }



        public
        void OnAnonymousInit() {

            if ( _pendingActions == null ) {

                _pendingActions     = new PActionQueue();
                _periodicalsDelay   = new WaitForSeconds( .55f );
            }
        }


        private WaitForSeconds      _periodicalsDelay;
        private PActionQueue        _pendingActions;

    }




}


