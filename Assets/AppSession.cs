using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PLAN;
//using PLAN.PDI;

using System;
//using System.Threading.Tasks;



public class AppSession : MonoBehaviour {


    // Set via GameObject "PSettings-main
    public
    PSettings                       Settings;



	// Use this for initialization
	void Awake() {

        Settings = new PSettings();
        Settings.MapToFile( "AppSession.json" );

        Settings.LoadFromStorage();



		Settings.store.Set( "/pdi/eth/some-uuid/datadir", "/Users/aomeara/Library/Application Support/PLAN/pdi/eth/geth/some-uuid/" );
        Settings.store.Set( "/pdi/eth/some-uuid/account", "0x05c50445814d905b772788f7b9da13b0206454ba" );     // sealer acct, pw: test


		StartCoroutine( MountHosts() );

        StartCoroutine( PRuntime.Instance.DoPeriodicals() );
	}



	void Start() {



	}








	void Update() {

		//_root.Delegate.PortalManager.FocusPortalSpace.FocusUpdate();
	
    }

    void OnApplicationQuit() {

        Settings.UpdateStorage();

        PRuntime.Instance.InitiateShutdown( null );;
        
    }
   

	IEnumerator MountHosts() {

        yield return null;

        /*
		// Below will be replaced by a config file that stores a list of the invocations of all the available data stores.
		// For example, the user could add an extra local storage option by adding "/plan/PDI/loplex/mydb" to this list.
		// For now, we just keep a manual list inlined here.
		var invocations = new List<DataHostInvocation> {
			new DataHostInvocation( DataHost_memplex.ProtocolRoot + "me" ),
			new DataHostInvocation( DataHost_memplex.ProtocolRoot + "plan-foundation" ),
			new DataHostInvocation( DataHost_eth.ProtocolRoot + "some-uuid" ),
		};

		// For each invocation we have, mount the DataHost that corresponds to it
		{
			yield return new WaitForSecondsRealtime(1.0f);

			foreach( var i in invocations ) {
				//yield return new WaitForSecondsRealtime(.1f);

				_DataHostManager.MountDataHost( i, ( inHost, inErr ) => {
					if ( inErr != null ) {
						Debug.Log( string.Format( "inErr: {0}", inErr.Code ) );
						Debug.DebugBreak();
					}
				} );
			}
		}*/
	}




}



