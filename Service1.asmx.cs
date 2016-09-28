using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Services;
using Oracle.DataAccess.Client;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;

namespace is2EXDBGET
{
	/// <summary>
	/// [is2EXDBGET]
	/// </summary>
	//--------------------------------------------------------------------------
	//  エコー金属殿向けＩＳ２サーバマスタの取得処理
	//--------------------------------------------------------------------------
	// 修正履歴
	//--------------------------------------------------------------------------
	// 20__.__.__ KCL）名前 ___修正内容__________
	//--------------------------------------------------------------------------
	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2EXDBGET")]

	public class Service1 : is2EXDBGET.CommService
	{
		//		private static string sCRLF = "\\r\\n";
		//		private static string sSepa = "|";
		//		private static string sKanma = ",";
		//		private static string sDbl = "\"";
		//		private static string sSng = "'";
		/// <summary>
		/// Ｏｒａｃｌｅのエクスポートファイル（圧縮・暗号化済）があるフォルダ
		/// </summary>
		//private static string sDumpFolder = @"D:\IS2EX\oradata";
//		private static string sDumpFolder = @"D:\IS2EX\oradata";
			
		public Service1()
		{
			//CODEGEN: この呼び出しは、ASP.NET Web サービス デザイナで必要です。
			InitializeComponent();

			connectService();
		}

		#region コンポーネント デザイナで生成されたコード 
		
		//Web サービス デザイナで必要です。
		private IContainer components = null;
				
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			System.Type type = System.Type.GetType("System.String");
			System.Configuration.AppSettingsReader config = new System.Configuration.AppSettingsReader();

			// ＤＢ定義
			string sSUser = "";
			string sSPass = "";
			string sSTns  = "";
			// ＳＶＲ内ＤＢアクセス定義
			sSUser = config.GetValue("user", type).ToString();
			sSPass = config.GetValue("pass", type).ToString();
			sSTns  = config.GetValue("data", type).ToString();
			sSvUser = new string[]{sSUser,sSPass,sSTns};
			sConn = "User Id="  + sSUser
				+ ";Password=" + sSPass
				+ ";Data Source=" + sSTns;
			sDumpFolder = config.GetValue("DataPath", type).ToString();
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion


		/*********************************************************************
		 * エコー金属ＡＭ１１マスタ取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] EX_AM11GET(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＡＭ１１マスタ取得処理開始");
			
//			// データ域
			string[] sPreEncData;
//			string[] sNormData;
			int iCnt;
			
//			if (sUser[0] == "")
//				sUser = sSvUser;

			sPreEncData = null;
//			sNormData	= null;
			ArrayList arList = new ArrayList();
			ArrayList arList2 = new ArrayList();
			ArrayList sList2 = new ArrayList();
			DateTime dtNow = DateTime.Now;
			string sEncChar = "";

			string[] sRet = new string[2];


			//string[] sList;
			// ＤＢ接続
			OracleConnection conn2 = null;
//			conn2 = connect2(sUser);
			conn2 = connect2(sSvUser);

			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			try
			{
				string cmdQuery = "SELECT  \n"
					+ "   TRIM(元着区分)   || '|' "
					+ "|| TRIM(登録連番)   || '|' "
					+ "|| TRIM(開始原票番号)     || '|' " 
					+ "|| TRIM(終了原票番号)  || '|' " 
					+ "|| 使用開始日     || '|' " 
					+ "|| 削除ＦＧ  || '|' " 
					+ "|| 登録日時  || '|' " 
					+ "|| 登録ＰＧ  || '|' "
					+ "|| 登録者  || '|' " 
					+ "|| 更新日時  || '|' " 
					+ "|| 更新ＰＧ  || '|' " 
					+ "|| 更新者 " 
					+ " FROM \"ＡＭ１１送り状番号\" \n"
					+ " ORDER BY \"元着区分\",\"登録連番\",\"開始原票番号\" \n"
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				while (reader.Read())
				{
					arList.Add(reader.GetString(0));
				}
				disposeReader(reader);
				reader = null;

				sRet = new string[arList.Count + 1];
				if (arList.Count == 0)
				{
					// データなしのログ
					logWriter(sUser, INF," 送信対象データはありませんでした。");
					sRet[0] = "送信対象データ無し";
					return sRet;
				}
				else
				{
					//データあり。
					iCnt = 1;
					
					IEnumerator enumList = arList.GetEnumerator();
					
					// 3DES 向けKEYの定義
					string key1 = "ziQCD4PpSltdFgwc";			//16文字(128ﾋﾞｯﾄ)
					string keyIV_w = "1VM8I3ex";				// 8文字(64ﾋﾞｯﾄ)
					byte[] DesIV = Encoding.UTF8.GetBytes(keyIV_w);
					string key2 = System.DateTime.Today.ToString("yyyyMMdd");	
					//					string DesKey3 = key1 + key2;
					string DesKey3 = key1;
					//					byte[] DesKey = Encoding.Unicode.GetBytes(DesKey3);
					byte[] DesKey = Encoding.UTF8.GetBytes(DesKey3);
					//
					iCnt = 0;
					 
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();

						// 文字列を byte 配列に変換します
						byte[] source = Encoding.UTF8.GetBytes(enumList.Current.ToString());
						
						// Triple DES のサービス プロバイダを生成します
						TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

						// 
						// 入出力用のストリームを生成します
						MemoryStream ms = new MemoryStream();
						CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor( DesKey, DesIV ),
							CryptoStreamMode.Write);

						// ストリームに暗号化するデータを書き込みます
						cs.Write(source, 0, source.Length);
						cs.Close();
							
						// 暗号化されたデータを byte 配列で取得します
						byte[] b暗号データ = ms.ToArray();
						ms.Close();
						
						// byte 配列を文字列に変換してARRAYにセットします
						sEncChar = "";
						for(int iCharCnt = 0; iCharCnt < b暗号データ.Length; iCharCnt++)
						{
							sEncChar += b暗号データ[iCharCnt].ToString("X2");
						}	
						arList2.Add(sEncChar);
						iCnt++;
					}
					//暗号化Array から
					sPreEncData = new string[arList2.Count];

					sPreEncData = (string[])arList2.ToArray(typeof(string));
					//					objPreEncData = arList2.ToArray();

					//					int iCnt2 = 0;
					//					IEnumerator enumList2 = arList2.GetEnumerator();
					//					while (enumList2.MoveNext())
					//					{
					//						sPreEncData[iCnt2] = enumList2.Current.ToString();
					//						iCnt2++;
					//					}
					//通常データのArray から
					sRet = new string[arList.Count + 1];
					IEnumerator enumList4 = arList2.GetEnumerator();
					sRet[0] = "正常終了";
					int iCnt3 = 1;
					while (enumList4.MoveNext())
					{
						sRet[iCnt3] = enumList4.Current.ToString();
						iCnt3++;
					}
					logWriter(sUser, INF, iCnt3 - 1 + " 件のデータ取得処理が終了しました。");

				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * エコー金属ＣＭ０７マスタ取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM07_cmdQuery 
			= "SELECT \n"
			+ " 年月日,稼働日ＦＧ,その他ＦＧ \n"
			+ ",削除ＦＧ,登録日時,登録ＰＧ,登録者,更新日時,更新ＰＧ,更新者 \n"
			+ " FROM \"ＣＭ０７稼働日\" \n"
			+ " ORDER BY 年月日 \n"
			;
		static string CM07_key1 = "qnqtrRSjb5lkIPdN";			//16文字(128ﾋﾞｯﾄ)
		static string CM07_keyIV_w = "roF0AUUj";				// 8文字(64ﾋﾞｯﾄ)
		[WebMethod]
		public String[] EX_CM07GET(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ０７マスタ取得処理開始");
			return EX_GetData(sUser, CM07_cmdQuery, CM07_key1, CM07_keyIV_w);
		}

		/*********************************************************************
		 * エコー金属ＣＭ１０マスタ取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM10_cmdQuery 
			= "SELECT \n"
			+ " 店所ＣＤ,店所名,店所正式名,\"メッセージ\",集約店ＣＤ,住所 \n"
			+ ",電話番号,ＦＡＸ番号,\"メールアドレス\",通知用アドレス１,通知用アドレス２ \n"
			+ ",地区１,地区２,契約書店所名称,契約書住所都道府県,契約書住所１,契約書住所２ \n"
			+ ",契約書郵便番号,契約書電話番号,契約書ＦＡＸ番号 \n"
			+ ",削除ＦＧ,登録日時,登録ＰＧ,登録者,更新日時,更新ＰＧ,更新者 \n"
			+ " FROM \"ＣＭ１０店所\" \n"
			+ " ORDER BY 店所ＣＤ \n"
			;
		static string CM10_key1 = "rToeXA39ylRR3Mas";			//16文字(128ﾋﾞｯﾄ)
		static string CM10_keyIV_w = "0drk5RbM";				// 8文字(64ﾋﾞｯﾄ)
		[WebMethod]
		public String[] EX_CM10GET(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ１０マスタ取得処理開始");
			return EX_GetData(sUser, CM10_cmdQuery, CM10_key1, CM10_keyIV_w);
		}

		/*********************************************************************
		 * エコー金属ＣＭ１１マスタ取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM11_cmdQuery 
			= "SELECT \n"
			+ " 集荷店ＣＤ,集約店ＣＤ,使用開始日 \n"
			+ ",削除ＦＧ,登録日時,登録ＰＧ,登録者,更新日時,更新ＰＧ,更新者 \n"
			+ " FROM \"ＣＭ１１集約店\" \n"
			+ " ORDER BY 集約店ＣＤ \n"
			;
		static string CM11_key1 = "MHKLH13BWtmeXz37";			//16文字(128ﾋﾞｯﾄ)
		static string CM11_keyIV_w = "ynLj2fuH";				// 8文字(64ﾋﾞｯﾄ)
		[WebMethod]
		public String[] EX_CM11GET(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ１１マスタ取得処理開始");
			return EX_GetData(sUser, CM11_cmdQuery, CM11_key1, CM11_keyIV_w);
		}

		/*********************************************************************
		 * エコー金属ＣＭ１２マスタ取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM12_cmdQuery 
			= "SELECT \n"
			+ " 都道府県ＣＤ,市区町村ＣＤ,都道府県名,市区町村名 \n"
			+ ",都道府県カナ名,市区町村カナ名,施行年月日,廃止年月日 \n"
			+ ",削除ＦＧ,登録日時,登録ＰＧ,登録者,更新日時,更新ＰＧ,更新者 \n"
			+ " FROM \"ＣＭ１２市区町村\" \n"
			+ " ORDER BY 都道府県ＣＤ, 市区町村ＣＤ \n"
			;
		static string CM12_key1 = "SI0DKFh1Hlvi84Pc";			//16文字(128ﾋﾞｯﾄ)
		static string CM12_keyIV_w = "8SLh2v9O";				// 8文字(64ﾋﾞｯﾄ)
		[WebMethod]
		public String[] EX_CM12GET(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ１２マスタ取得処理開始");
			return EX_GetData(sUser, CM12_cmdQuery, CM12_key1, CM12_keyIV_w);
		}

//		/*********************************************************************
//		 * エコー金属ＣＭ１３マスタ取得処理
//		 * 引数：
//		 * 戻値：ステータス
//		 *
//		 *********************************************************************/
//		static string CM13_FileName 
//			= @"IS2EX_CM13"
//			;
//		[WebMethod]
//		public byte[] EX_CM13GET(string[] sUser)
//		{
//			logWriter(sUser, INF, "エコー金属ＣＭ１３マスタ取得処理開始");
//			return EX_GetFile(sUser, CM13_FileName);
//		}
//
//		/*********************************************************************
//		 * エコー金属ＣＭ１４マスタ取得処理
//		 * 引数：
//		 * 戻値：ステータス
//		 *
//		 *********************************************************************/
//		static string CM14_FileName 
//			= @"IS2EX_CM14"
//			;
//		[WebMethod]
//		public byte[] EX_CM14GET(string[] sUser)
//		{
//			logWriter(sUser, INF, "エコー金属ＣＭ１４マスタ取得処理開始");
//			return EX_GetFile(sUser, CM14_FileName);
//		}
//
		/*********************************************************************
		 * エコー金属ＣＭ１３マスタ取得処理(String)
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM13_FileNameS 
			= @"IS2EX_CM13"
			;
		[WebMethod]
		public string[] EX_CM13GETS(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ１３マスタ取得処理開始");
			return EX_GetFileS(sUser, CM13_FileNameS);
		}

		/*********************************************************************
		 * エコー金属ＣＭ１４マスタ取得処理(String)
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM14_FileNameS 
			= @"IS2EX_CM14"
			;
		[WebMethod]
		public string[] EX_CM14GETS(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ１４マスタ取得処理開始");
			return EX_GetFileS(sUser, CM14_FileNameS);
		}

		/*********************************************************************
		 * エコー金属ＣＭ１５マスタ取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM15_cmdQuery 
			= "SELECT \n"
			+ " 郵便番号 \n"
			+ ",削除ＦＧ,登録日時,登録ＰＧ,登録者,更新日時,更新ＰＧ,更新者 \n"
			+ " FROM \"ＣＭ１５着店非表示\" \n"
			+ " ORDER BY 郵便番号 \n"
			;
		static string CM15_key1 = "KNbVGgohMeYeI2st";			//16文字(128ﾋﾞｯﾄ)
		static string CM15_keyIV_w = "hNBZV73C";				// 8文字(64ﾋﾞｯﾄ)

		[WebMethod]
		public String[] EX_CM15GET(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ１５マスタ取得処理開始");
			return EX_GetData(sUser, CM15_cmdQuery, CM15_key1, CM15_keyIV_w);
		}

		/*********************************************************************
		 * エコー金属ＣＭ１７マスタ取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM17_cmdQuery_SELECT 
			= "SELECT \n"
			+ " 発店所ＣＤ,着店所ＣＤ,仕分ＣＤ \n"
			+ ",削除ＦＧ,登録日時,登録ＰＧ,登録者,更新日時,更新ＰＧ,更新者 \n"
			+ " FROM \"ＣＭ１７仕分\" \n"
			;
		static string CM17_cmdQuery_ORDER
			= " ORDER BY 発店所ＣＤ,着店所ＣＤ \n"
			;
		static string CM17_key1 = "GKv6R8ei5HofV3yx";			//16文字(128ﾋﾞｯﾄ)
		static string CM17_keyIV_w = "E93fgcYu";				// 8文字(64ﾋﾞｯﾄ)
		[WebMethod]
//		public String[] EX_CM17GET(string[] sUser)
		public String[] EX_CM17GET(string[] sUser,string[] s会員, string sZenUpdate)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ１７マスタ取得処理開始");
			string CM17_cmdQuery = CM17_cmdQuery_SELECT;

			// 同期更新日時の取得
			CM17_cmdQuery += "WHERE 更新日時 >= "+ sZenUpdate +" \n";

			CM17_cmdQuery += CM17_cmdQuery_ORDER;
			string[] sRetWeb = EX_GetData(sUser, CM17_cmdQuery, CM17_key1, CM17_keyIV_w);

//			// 同期更新日時の更新
//			if(sRetWeb[0].Length == 4 && sRetDate[2].Length > 1){
//				logWriter(sUser, INF, "今回同期日時["+sRetDate[2]+"]");
//				string[] sRet = EX_XM01_SetUpdDate(sUser,"echo","CM17",sRetDate[2]);
//			}
			if(sRetWeb[0].Equals("送信対象データ無し")){
				sRetWeb[0] = "該当無し";
			}
			return sRetWeb;
		}

		/*********************************************************************
		 * エコー金属ＣＭ１８マスタ取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM18_cmdQuery 
			= "SELECT \n"
			+ " 登録日,\"シーケンスＮＯ\",優先順,管理者区分,表題,詳細内容 \n"
			+ ",ボタン名,アドレス,店所ＣＤ,会員ＣＤ,\"メッセージ\" \n"
			+ ",表示期間開始,表示期間終了,表示ＦＧ \n"
			+ ",削除ＦＧ,登録日時,登録ＰＧ,登録者,更新日時,更新ＰＧ,更新者 \n"
			+ " FROM \"ＣＭ１８お知らせ\" \n"
			+ " ORDER BY 登録日,\"シーケンスＮＯ\" \n"
			;
		static string CM18_key1 = "0e1kZgvPiZMAqSUk";			//16文字(128ﾋﾞｯﾄ)
		static string CM18_keyIV_w = "jE3Z5sd2";				// 8文字(64ﾋﾞｯﾄ)

		[WebMethod]
		public String[] EX_CM18GET(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ１８マスタ取得処理開始");
			return EX_GetData(sUser, CM18_cmdQuery, CM18_key1, CM18_keyIV_w);
		}

		/*********************************************************************
		 * エコー金属ＣＭ１９マスタ取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string CM19_cmdQuery 
			= "SELECT \n"
			+ " 郵便番号,住所,住所ＣＤ,店所ＣＤ \n"
			+ ",削除ＦＧ,登録日時,登録ＰＧ,登録者,更新日時,更新ＰＧ,更新者 \n"
			+ " FROM \"ＣＭ１９郵便住所\" \n"
			+ " ORDER BY 郵便番号,住所,住所ＣＤ,店所ＣＤ \n"
			;
		static string CM19_key1 = "YZV4EfVwT63CwpRp";			//16文字(128ﾋﾞｯﾄ)
		static string CM19_keyIV_w = "b7JFvHaS";				// 8文字(64ﾋﾞｯﾄ)

		[WebMethod]
		public String[] EX_CM19GET(string[] sUser)
		{
			logWriter(sUser, INF, "エコー金属ＣＭ１９マスタ取得処理開始");
			return EX_GetData(sUser, CM19_cmdQuery, CM19_key1, CM19_keyIV_w);
		}

		/*********************************************************************
		 * エコー金属マスタ取得共通処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		private String[] EX_GetData(string[] sUser, string cmdQuery, string key1, string keyIV_w)
		{
			// データ域
			string[] sPreEncData;
			int iCnt;

			sPreEncData = null;
			ArrayList arList = new ArrayList();
			ArrayList arList2 = new ArrayList();
			ArrayList sList2 = new ArrayList();
			string sEncChar = "";

			string[] sRet = new string[2];

			// ＤＢ接続
			OracleConnection conn2 = null;
			conn2 = connect2(sSvUser);

			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			try{
				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				StringBuilder sbBuffer = new StringBuilder(4096);
				while(reader.Read())
				{
					sbBuffer = new StringBuilder(4096);
					for(int iCntCol = 0; iCntCol < reader.FieldCount; iCntCol++)
					{
						if(iCntCol > 0){
							sbBuffer.Append(",");
						}
						if(reader.GetValue(iCntCol) is System.String)
						{
							sbBuffer.Append("\'");
							sbBuffer.Append(reader.GetString(iCntCol));
							sbBuffer.Append("\'");
						}
						else
						{
							sbBuffer.Append(reader.GetDecimal(iCntCol).ToString());
						}
					}
					arList.Add(sbBuffer.ToString());
					sbBuffer = null;
				}
				disposeReader(reader);
				reader = null;

				sRet = new string[arList.Count + 1];
				if(arList.Count == 0)
				{
					// データなしのログ
					logWriter(sUser, INF," 送信対象データはありませんでした。");
					sRet[0] = "送信対象データ無し";
					return sRet;
				}
				else
				{
					//データあり。
					iCnt = 1;
					
					logWriter(sUser, INF," データ変換開始");

					IEnumerator enumList = arList.GetEnumerator();
					
					// 3DES 向けKEYの定義
					byte[] DesIV = Encoding.UTF8.GetBytes(keyIV_w);
					string DesKey3 = key1;
					byte[] DesKey = Encoding.UTF8.GetBytes(DesKey3);
					//
					iCnt = 0;
					 
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();

						// 文字列を byte 配列に変換します
						byte[] source = Encoding.UTF8.GetBytes(enumList.Current.ToString());
						
						// Triple DES のサービス プロバイダを生成します
						TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

						// 
						// 入出力用のストリームを生成します
						MemoryStream ms = new MemoryStream();
						CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor( DesKey, DesIV ),
							CryptoStreamMode.Write);

						// ストリームに暗号化するデータを書き込みます
						cs.Write(source, 0, source.Length);
						cs.Close();
							
						// 暗号化されたデータを byte 配列で取得します
						byte[] b暗号データ = ms.ToArray();
						ms.Close();
						
						// byte 配列を文字列に変換してARRAYにセットします
						sEncChar = "";
						for(int iCharCnt = 0; iCharCnt < b暗号データ.Length; iCharCnt++)
						{
							sEncChar += b暗号データ[iCharCnt].ToString("X2");
						}	
						arList2.Add(sEncChar);
						iCnt++;
					}
					//暗号化Array から
					sPreEncData = new string[arList2.Count];

					sPreEncData = (string[])arList2.ToArray(typeof(string));
					//通常データのArray から
					sRet = new string[arList.Count + 1];
					IEnumerator enumList4 = arList2.GetEnumerator();
					sRet[0] = "正常終了";
					int iCnt3 = 1;
					while (enumList4.MoveNext())
					{
						sRet[iCnt3] = enumList4.Current.ToString();
						iCnt3++;
					}
					logWriter(sUser, INF, iCnt3 - 1 + " 件のデータ取得処理が終了しました。");
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * エコー金属ファイル取得共通処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		private byte[] EX_GetFile(string[] sUser, string sFile)
		{
			byte[] bRet = null;
			FileStream fs = null;
			try{
				fs = new FileStream( sFile, FileMode.Open, FileAccess.Read ); 
				fs.Seek( 0, System.IO.SeekOrigin.Begin ); 
				int size = (int)fs.Length; 
				byte[] buf = new byte[size]; 
				fs.Read( buf, 0, buf.Length ); 
				fs.Close();
				fs = null;
				bRet = buf;
			}catch(Exception){
				;
			}finally{
				if(fs != null) fs.Close();
			}
			return bRet;
		}

		/*********************************************************************
		 * エコー金属ファイル取得共通処理(String)
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		private String[] EX_GetFileS(string[] sUser, string sFile)
		{

			string [] sResult = new string[1];
			string sFileData = string.Empty;
			string sErrMsg = string.Empty;
			// ダンプファイルパス
			string sDumpFile = Path.Combine(sDumpFolder, sFile + ".enc");
//			// フラグファイルパス
//			string sFlagFile = Path.Combine(sDumpFolder, sFile + ".sts");

//			// フラグファイルの存在チェック
//			if (! File.Exists(sFlagFile))
//			{
//				sErrMsg = string.Format("フラグファイルがありません。[{0}]", sFlagFile);
//				logWriter(sUser, ERR, sErrMsg);
//				return new string [] {sErrMsg};
//			}

			// ファイルの存在チェック
			if (! File.Exists(sDumpFile))
			{
				sErrMsg = string.Format("データファイルがありません。[{0}]", sDumpFile);
				logWriter(sUser, ERR, sErrMsg);
				return new string [] {sErrMsg};
			}

			try 
			{
				// ファイルを読み込む
				using (StreamReader sr = new StreamReader(sDumpFile, Encoding.GetEncoding("shift_jis")))
				{
					sFileData = sr.ReadToEnd();
				}
			} 
			catch (Exception ex) 
			{
				sErrMsg = string.Format("ファイルの読み取りに失敗しました。[{0}] - {1}", sDumpFile, ex.Message);
				logWriter(sUser, ERR, sErrMsg);
				return new string [] {sErrMsg};
			}

//			// フラグファイル削除
//			if (File.Exists(sFlagFile)) File.Delete(sFlagFile);

			// 戻り値の準備
			sResult = new string[2];
			sResult[0] = "正常終了";
			sResult[1] = sFileData;

			return sResult;
			
		}

		/*********************************************************************
		 * エコー金属テーブル同期更新日時取得処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] EX_XM01_GetUpdDate(string[] sUser, string sKaiinCD, string sTableKey)
		{
			DateTime dtNow = DateTime.Now;
			string[] sRet = new string[3];

			// ＤＢ接続
			OracleConnection conn2 = null;
			conn2 = connect2(sSvUser);

			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			try{
				string cmdQuery = "SELECT ＤＢ更新日時 \n"
								+ " FROM ＸＭ０１更新日時 \n "
								+ " WHERE 会員ＣＤ = '"+sKaiinCD+"' \n"
								+ " AND \"ＤＢキー\" = '"+sTableKey+"' \n"
								+ " AND 削除ＦＧ = '0' \n"
								;
				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				if(reader.Read())
				{
					sRet[0] = "正常終了";
					sRet[1] = reader.GetDecimal(0).ToString();
				}
				else
				{
					sRet[0] = "該当無し";
					sRet[1] = "1";
				}
				disposeReader(reader);
				reader = null;

				cmdQuery = "SELECT TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " FROM DUAL \n "
						;
				reader = CmdSelect(sUser, conn2, cmdQuery);
				if(reader.Read())
				{
					sRet[2] = reader.GetString(0);
				}
				else
				{
					sRet[2] = "";
				}
				disposeReader(reader);
				reader = null;
//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * エコー金属テーブル同期更新日時更新処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] EX_XM01_SetUpdDate(string[] sUser, string[] sKaiinCD, string sTableKey, string sUpdDate, string sUpdPG, string sUpdSya)
		{
			string[] sRet = new string[3];

			// ＤＢ接続
			OracleConnection conn2 = null;
			conn2 = connect2(sSvUser);

			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();
			try{
				int iCnt = 1;
				while (iCnt < sKaiinCD.Length)
				{
					string cmdQuery = "UPDATE ＸＭ０１更新日時 SET \n "
								+ " ＤＢ更新日時 = "+sUpdDate+"\n"
								+ ", 更新日時 =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
								+ ",更新ＰＧ  = '" + sUpdPG +"'"
								+ ",更新者    = '" + sUpdSya +"' \n"
								+ " WHERE 会員ＣＤ = '"+sKaiinCD[iCnt]+"' \n"
								+ " AND \"ＤＢキー\" = '"+sTableKey+"' \n"
								+ " AND 削除ＦＧ = '0' \n"
								;
					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					if(iUpdRow == 0)
					{
						cmdQuery = "INSERT INTO ＸＭ０１更新日時( \n "
								+ " 会員ＣＤ,ＤＢキー,ＤＢ更新日時,ＤＢ更新日付 \n"
								+ ",削除ＦＧ \n"
								+ ",登録日時, 登録ＰＧ, 登録者 \n"
								+ ",更新日時, 更新ＰＧ, 更新者 \n"
								+ ")VALUES( \n"
								+ " '"+sKaiinCD[iCnt]+"' \n"
								+ ",'"+sTableKey+"' \n"
								+ ", "+sUpdDate+" \n"
								+ ", 0 \n"
								+ ", '0' \n"
								+ ", TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sUpdPG +"','" + sUpdSya +"' \n"	//登録日時～
								+ ", TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sUpdPG +"','" + sUpdSya +"' \n"   //更新日時～		
								+ " ) \n"
								;
						iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					}
					if(iUpdRow == 1)
					{
						sRet[0] = "正常終了";
					}
					else
					{
						sRet[0] = "登録エラー";
					}
					iCnt++;
				}

				tran.Commit();
//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * エコー金属ＳＭ０１マスタ更新処理
		 * 引数：
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		static string SM01_key1 = "rTqpnRaWRNjrl6e8";			//16文字(128ﾋﾞｯﾄ)
		static string SM01_keyIV_w = "BN3bT2q3";				// 8文字(64ﾋﾞｯﾄ)

		string sSM01_テーブル名 = "\"ＳＭ０１荷送人\"";
		string sSM01_項目名 
			= " 会員ＣＤ,部門ＣＤ,荷送人ＣＤ \n"
			+ ",得意先ＣＤ,得意先部課ＣＤ \n"
			+ ",電話番号１,電話番号２,電話番号３,ＦＡＸ番号１,ＦＡＸ番号２,ＦＡＸ番号３ \n"
			+ ",住所１,住所２,住所３,名前１,名前２,名前３ \n"
			+ ",郵便番号,カナ略称,才数,重量,荷札区分,\"メールアドレス\" \n"
			+ ",削除ＦＧ,登録日時,登録ＰＧ,登録者,更新日時,更新ＰＧ,更新者 \n"
			;

		[WebMethod]
		public string[] EX_SM01SET(string[] sUser, string[] sGetData)
		{
			logWriter(sUser, INF, "エコー金属ＳＭ０１マスタ更新処理開始");

			string[] sRet = {"","",""};
			int in_cnt = 0;

			//---- 暗号化ファイルの復号
			byte[] DesIV = Encoding.UTF8.GetBytes(SM01_keyIV_w);
			string DesKey3 = SM01_key1;
			byte[] DesKey = Encoding.UTF8.GetBytes(DesKey3);

			byte[] bText;
			string sText;
				
			ArrayList aData = new ArrayList();
			for (in_cnt = 1; in_cnt < sGetData.Length; in_cnt++)
			{
				string  sByte = "";
				sText = sGetData[in_cnt];

				bText = new byte[sText.Length / 2];
				for(int iCnt = 0; iCnt < sText.Length; iCnt+=2)
				{
					sByte = sText.Substring(iCnt, 2);
					bText[iCnt/2] = Convert.ToByte(sByte,16);
				}

				// Trippe DES のサービス プロバイダを生成します
				TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

				// 入出力用のストリームを生成します(復号)
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor( DesKey, DesIV),
				CryptoStreamMode.Write);

				// ストリームに暗号化されたデータを書き込みます
				cs.Write(bText, 0, bText.Length);
				cs.Close();

				// 復号化されたデータを byte 配列で取得します
				byte[] destination = ms.ToArray();
				ms.Close();

				// byte 配列を文字列に変換してARRAYに保存する
				aData.Add(Encoding.UTF8.GetString(destination));
				// 復号化された1行を溜め込む　
			}

			//----　ＤＢ処理
			OracleConnection conn2 = null;

			// ＤＢ接続
			conn2 = connect2(sSvUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
	
			OracleTransaction tran;
			tran = conn2.BeginTransaction();
		
			try
			{
//				// テーブル全件削除
//				string cmdQuery = "DELETE FROM " + sテーブル名;
//				int iDelRtn = CmdUpdate(sMyUser, conn2, cmdQuery);

				string[] s項目名 = sSM01_項目名.Split(',');
				StringBuilder sbBuffer;
				IEnumerator enumList = aData.GetEnumerator();
				int i更新件数 = 0;
				int i登録件数 = 0;
				while(enumList.MoveNext())
				{
					string sStr = enumList.Current.ToString();
					string[] s項目 = sStr.Split(',');
					sbBuffer = new StringBuilder(4096);
					sbBuffer.Append("UPDATE "+ sSM01_テーブル名 + " SET \n");
					for(int iCnt = 3; iCnt < s項目名.Length; iCnt++){
						if(iCnt == 3){
							sbBuffer.Append(" "+s項目名[iCnt]+" = "+ s項目[iCnt] + " \n");
						}else{
							sbBuffer.Append(","+s項目名[iCnt]+" = "+ s項目[iCnt] + " \n");
						}
					}
					sbBuffer.Append("WHERE 会員ＣＤ = " + s項目[0] + " \n");
					sbBuffer.Append("AND 部門ＣＤ = " + s項目[1] + " \n");
					sbBuffer.Append("AND 荷送人ＣＤ = " + s項目[2] + " \n");
					int iUpdRow = CmdUpdate(sUser, conn2, sbBuffer);
					if(iUpdRow == 0){
						sbBuffer = new StringBuilder(4096);
						sbBuffer.Append("INSERT INTO "+ sSM01_テーブル名 + "( \n");
						sbBuffer.Append(sSM01_項目名);
						sbBuffer.Append(")VALUES( \n");
						sbBuffer.Append(sStr);
						sbBuffer.Append(") \n");

						iUpdRow = CmdUpdate(sUser, conn2, sbBuffer);
						i登録件数 += iUpdRow;
					}else{
						i更新件数 += iUpdRow;
					}
				}
				tran.Commit();
				sRet[0] = "正常終了";
				sRet[1] = i登録件数.ToString();
				sRet[2] = i更新件数.ToString();
				logWriter(sUser, INF, " 登録件数："+sRet[1]);
				logWriter(sUser, INF, " 更新件数："+sRet[2]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
	}
}
