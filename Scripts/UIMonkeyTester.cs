using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kogane
{
	/// <summary>
	/// UI のモンキーテストを行うためのクラス
	/// </summary>
	public sealed class UIMonkeyTester
	{
		//================================================================================
		// 変数(readonly)
		//================================================================================
		private readonly Predicate<GameObject>                         m_gameObjectFilter;
		private readonly Action<IEventSystemHandler, PointerEventData> m_onClick;

		private readonly List<RaycastResult> m_raycastResults = new List<RaycastResult>();
		private readonly List<Transform>     m_transforms     = new List<Transform>();

		//================================================================================
		// 関数
		//================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UIMonkeyTester
		(
			Predicate<GameObject>                         gameObjectFilter,
			Action<IEventSystemHandler, PointerEventData> onClick
		)
		{
			m_gameObjectFilter = gameObjectFilter;
			m_onClick          = onClick;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UIMonkeyTester( Predicate<GameObject> gameObjectFilter )
			: this
			(
				gameObjectFilter: gameObjectFilter,
				onClick: ( handler, data ) => ( handler as IPointerClickHandler )?.OnPointerClick( data )
			)
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UIMonkeyTester()
			: this( null )
		{
		}

		/// <summary>
		/// 指定された座標をクリックします
		/// </summary>
		public void Click( float x, float y )
		{
			var position = new Vector2( x, y );

			Click( position );
		}

		/// <summary>
		/// 指定された座標をクリックします
		/// </summary>
		public void Click( Vector2 position )
		{
			// 指定された座標に対するマウス・タッチイベントを作成します
			var pointerEventData = new PointerEventData( EventSystem.current )
			{
				position = position,
			};

			m_raycastResults.Clear();

			// 指定された座標に存在する UI オブジェクトをすべて取得します
			EventSystem.current.RaycastAll( pointerEventData, m_raycastResults );

			if ( m_raycastResults.Count <= 0 ) return;

			// 配列の一番先頭が一番手前に存在する UI オブジェクトの情報なので取得します
			var raycastResult  = m_raycastResults[ 0 ];
			var rootGameObject = raycastResult.gameObject;

			if ( rootGameObject == null ) return;

			// その UI オブジェクトとすべての親をリストに格納します
			var parent = rootGameObject.transform;

			m_transforms.Clear();

			while ( parent != null )
			{
				m_transforms.Add( parent );
				parent = parent.parent;
			}

			// 子から順番にクリックしていきます
			// クリックした時にイベントが発火するオブジェクトが見つかるまで順番にくりっくしていきます
			// イベントが発火した場合はそこで処理を終了します
			for ( var i = 0; i < m_transforms.Count; i++ )
			{
				var transform  = m_transforms[ i ];
				var gameObject = transform.gameObject;

				if ( m_gameObjectFilter != null && !m_gameObjectFilter( gameObject ) ) continue;

				var isSuccess = ExecuteEvents.Execute<IEventSystemHandler>
				(
					target: gameObject,
					eventData: pointerEventData,
					functor: ( handler, _ ) => m_onClick( handler, pointerEventData )
				);

				if ( isSuccess ) break;
			}
		}
	}
}