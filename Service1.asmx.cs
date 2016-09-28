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
	//  �G�R�[�����a�����h�r�Q�T�[�o�}�X�^�̎擾����
	//--------------------------------------------------------------------------
	// �C������
	//--------------------------------------------------------------------------
	// 20__.__.__ KCL�j���O ___�C�����e__________
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
		/// �n�����������̃G�N�X�|�[�g�t�@�C���i���k�E�Í����ρj������t�H���_
		/// </summary>
		//private static string sDumpFolder = @"D:\IS2EX\oradata";
//		private static string sDumpFolder = @"D:\IS2EX\oradata";
			
		public Service1()
		{
			//CODEGEN: ���̌Ăяo���́AASP.NET Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
			InitializeComponent();

			connectService();
		}

		#region �R���|�[�l���g �f�U�C�i�Ő������ꂽ�R�[�h 
		
		//Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
		private IContainer components = null;
				
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
			System.Type type = System.Type.GetType("System.String");
			System.Configuration.AppSettingsReader config = new System.Configuration.AppSettingsReader();

			// �c�a��`
			string sSUser = "";
			string sSPass = "";
			string sSTns  = "";
			// �r�u�q���c�a�A�N�Z�X��`
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
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
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
		 * �G�R�[�����`�l�P�P�}�X�^�擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] EX_AM11GET(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����`�l�P�P�}�X�^�擾�����J�n");
			
//			// �f�[�^��
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
			// �c�a�ڑ�
			OracleConnection conn2 = null;
//			conn2 = connect2(sUser);
			conn2 = connect2(sSvUser);

			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			try
			{
				string cmdQuery = "SELECT  \n"
					+ "   TRIM(�����敪)   || '|' "
					+ "|| TRIM(�o�^�A��)   || '|' "
					+ "|| TRIM(�J�n���[�ԍ�)     || '|' " 
					+ "|| TRIM(�I�����[�ԍ�)  || '|' " 
					+ "|| �g�p�J�n��     || '|' " 
					+ "|| �폜�e�f  || '|' " 
					+ "|| �o�^����  || '|' " 
					+ "|| �o�^�o�f  || '|' "
					+ "|| �o�^��  || '|' " 
					+ "|| �X�V����  || '|' " 
					+ "|| �X�V�o�f  || '|' " 
					+ "|| �X�V�� " 
					+ " FROM \"�`�l�P�P�����ԍ�\" \n"
					+ " ORDER BY \"�����敪\",\"�o�^�A��\",\"�J�n���[�ԍ�\" \n"
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
					// �f�[�^�Ȃ��̃��O
					logWriter(sUser, INF," ���M�Ώۃf�[�^�͂���܂���ł����B");
					sRet[0] = "���M�Ώۃf�[�^����";
					return sRet;
				}
				else
				{
					//�f�[�^����B
					iCnt = 1;
					
					IEnumerator enumList = arList.GetEnumerator();
					
					// 3DES ����KEY�̒�`
					string key1 = "ziQCD4PpSltdFgwc";			//16����(128�ޯ�)
					string keyIV_w = "1VM8I3ex";				// 8����(64�ޯ�)
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

						// ������� byte �z��ɕϊ����܂�
						byte[] source = Encoding.UTF8.GetBytes(enumList.Current.ToString());
						
						// Triple DES �̃T�[�r�X �v���o�C�_�𐶐����܂�
						TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

						// 
						// ���o�͗p�̃X�g���[���𐶐����܂�
						MemoryStream ms = new MemoryStream();
						CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor( DesKey, DesIV ),
							CryptoStreamMode.Write);

						// �X�g���[���ɈÍ�������f�[�^���������݂܂�
						cs.Write(source, 0, source.Length);
						cs.Close();
							
						// �Í������ꂽ�f�[�^�� byte �z��Ŏ擾���܂�
						byte[] b�Í��f�[�^ = ms.ToArray();
						ms.Close();
						
						// byte �z��𕶎���ɕϊ�����ARRAY�ɃZ�b�g���܂�
						sEncChar = "";
						for(int iCharCnt = 0; iCharCnt < b�Í��f�[�^.Length; iCharCnt++)
						{
							sEncChar += b�Í��f�[�^[iCharCnt].ToString("X2");
						}	
						arList2.Add(sEncChar);
						iCnt++;
					}
					//�Í���Array ����
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
					//�ʏ�f�[�^��Array ����
					sRet = new string[arList.Count + 1];
					IEnumerator enumList4 = arList2.GetEnumerator();
					sRet[0] = "����I��";
					int iCnt3 = 1;
					while (enumList4.MoveNext())
					{
						sRet[iCnt3] = enumList4.Current.ToString();
						iCnt3++;
					}
					logWriter(sUser, INF, iCnt3 - 1 + " ���̃f�[�^�擾�������I�����܂����B");

				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �G�R�[�����b�l�O�V�}�X�^�擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM07_cmdQuery 
			= "SELECT \n"
			+ " �N����,�ғ����e�f,���̑��e�f \n"
			+ ",�폜�e�f,�o�^����,�o�^�o�f,�o�^��,�X�V����,�X�V�o�f,�X�V�� \n"
			+ " FROM \"�b�l�O�V�ғ���\" \n"
			+ " ORDER BY �N���� \n"
			;
		static string CM07_key1 = "qnqtrRSjb5lkIPdN";			//16����(128�ޯ�)
		static string CM07_keyIV_w = "roF0AUUj";				// 8����(64�ޯ�)
		[WebMethod]
		public String[] EX_CM07GET(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�O�V�}�X�^�擾�����J�n");
			return EX_GetData(sUser, CM07_cmdQuery, CM07_key1, CM07_keyIV_w);
		}

		/*********************************************************************
		 * �G�R�[�����b�l�P�O�}�X�^�擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM10_cmdQuery 
			= "SELECT \n"
			+ " �X���b�c,�X����,�X��������,\"���b�Z�[�W\",�W��X�b�c,�Z�� \n"
			+ ",�d�b�ԍ�,�e�`�w�ԍ�,\"���[���A�h���X\",�ʒm�p�A�h���X�P,�ʒm�p�A�h���X�Q \n"
			+ ",�n��P,�n��Q,�_�񏑓X������,�_�񏑏Z���s���{��,�_�񏑏Z���P,�_�񏑏Z���Q \n"
			+ ",�_�񏑗X�֔ԍ�,�_�񏑓d�b�ԍ�,�_�񏑂e�`�w�ԍ� \n"
			+ ",�폜�e�f,�o�^����,�o�^�o�f,�o�^��,�X�V����,�X�V�o�f,�X�V�� \n"
			+ " FROM \"�b�l�P�O�X��\" \n"
			+ " ORDER BY �X���b�c \n"
			;
		static string CM10_key1 = "rToeXA39ylRR3Mas";			//16����(128�ޯ�)
		static string CM10_keyIV_w = "0drk5RbM";				// 8����(64�ޯ�)
		[WebMethod]
		public String[] EX_CM10GET(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�P�O�}�X�^�擾�����J�n");
			return EX_GetData(sUser, CM10_cmdQuery, CM10_key1, CM10_keyIV_w);
		}

		/*********************************************************************
		 * �G�R�[�����b�l�P�P�}�X�^�擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM11_cmdQuery 
			= "SELECT \n"
			+ " �W�דX�b�c,�W��X�b�c,�g�p�J�n�� \n"
			+ ",�폜�e�f,�o�^����,�o�^�o�f,�o�^��,�X�V����,�X�V�o�f,�X�V�� \n"
			+ " FROM \"�b�l�P�P�W��X\" \n"
			+ " ORDER BY �W��X�b�c \n"
			;
		static string CM11_key1 = "MHKLH13BWtmeXz37";			//16����(128�ޯ�)
		static string CM11_keyIV_w = "ynLj2fuH";				// 8����(64�ޯ�)
		[WebMethod]
		public String[] EX_CM11GET(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�P�P�}�X�^�擾�����J�n");
			return EX_GetData(sUser, CM11_cmdQuery, CM11_key1, CM11_keyIV_w);
		}

		/*********************************************************************
		 * �G�R�[�����b�l�P�Q�}�X�^�擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM12_cmdQuery 
			= "SELECT \n"
			+ " �s���{���b�c,�s�撬���b�c,�s���{����,�s�撬���� \n"
			+ ",�s���{���J�i��,�s�撬���J�i��,�{�s�N����,�p�~�N���� \n"
			+ ",�폜�e�f,�o�^����,�o�^�o�f,�o�^��,�X�V����,�X�V�o�f,�X�V�� \n"
			+ " FROM \"�b�l�P�Q�s�撬��\" \n"
			+ " ORDER BY �s���{���b�c, �s�撬���b�c \n"
			;
		static string CM12_key1 = "SI0DKFh1Hlvi84Pc";			//16����(128�ޯ�)
		static string CM12_keyIV_w = "8SLh2v9O";				// 8����(64�ޯ�)
		[WebMethod]
		public String[] EX_CM12GET(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�P�Q�}�X�^�擾�����J�n");
			return EX_GetData(sUser, CM12_cmdQuery, CM12_key1, CM12_keyIV_w);
		}

//		/*********************************************************************
//		 * �G�R�[�����b�l�P�R�}�X�^�擾����
//		 * �����F
//		 * �ߒl�F�X�e�[�^�X
//		 *
//		 *********************************************************************/
//		static string CM13_FileName 
//			= @"IS2EX_CM13"
//			;
//		[WebMethod]
//		public byte[] EX_CM13GET(string[] sUser)
//		{
//			logWriter(sUser, INF, "�G�R�[�����b�l�P�R�}�X�^�擾�����J�n");
//			return EX_GetFile(sUser, CM13_FileName);
//		}
//
//		/*********************************************************************
//		 * �G�R�[�����b�l�P�S�}�X�^�擾����
//		 * �����F
//		 * �ߒl�F�X�e�[�^�X
//		 *
//		 *********************************************************************/
//		static string CM14_FileName 
//			= @"IS2EX_CM14"
//			;
//		[WebMethod]
//		public byte[] EX_CM14GET(string[] sUser)
//		{
//			logWriter(sUser, INF, "�G�R�[�����b�l�P�S�}�X�^�擾�����J�n");
//			return EX_GetFile(sUser, CM14_FileName);
//		}
//
		/*********************************************************************
		 * �G�R�[�����b�l�P�R�}�X�^�擾����(String)
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM13_FileNameS 
			= @"IS2EX_CM13"
			;
		[WebMethod]
		public string[] EX_CM13GETS(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�P�R�}�X�^�擾�����J�n");
			return EX_GetFileS(sUser, CM13_FileNameS);
		}

		/*********************************************************************
		 * �G�R�[�����b�l�P�S�}�X�^�擾����(String)
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM14_FileNameS 
			= @"IS2EX_CM14"
			;
		[WebMethod]
		public string[] EX_CM14GETS(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�P�S�}�X�^�擾�����J�n");
			return EX_GetFileS(sUser, CM14_FileNameS);
		}

		/*********************************************************************
		 * �G�R�[�����b�l�P�T�}�X�^�擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM15_cmdQuery 
			= "SELECT \n"
			+ " �X�֔ԍ� \n"
			+ ",�폜�e�f,�o�^����,�o�^�o�f,�o�^��,�X�V����,�X�V�o�f,�X�V�� \n"
			+ " FROM \"�b�l�P�T���X��\��\" \n"
			+ " ORDER BY �X�֔ԍ� \n"
			;
		static string CM15_key1 = "KNbVGgohMeYeI2st";			//16����(128�ޯ�)
		static string CM15_keyIV_w = "hNBZV73C";				// 8����(64�ޯ�)

		[WebMethod]
		public String[] EX_CM15GET(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�P�T�}�X�^�擾�����J�n");
			return EX_GetData(sUser, CM15_cmdQuery, CM15_key1, CM15_keyIV_w);
		}

		/*********************************************************************
		 * �G�R�[�����b�l�P�V�}�X�^�擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM17_cmdQuery_SELECT 
			= "SELECT \n"
			+ " ���X���b�c,���X���b�c,�d���b�c \n"
			+ ",�폜�e�f,�o�^����,�o�^�o�f,�o�^��,�X�V����,�X�V�o�f,�X�V�� \n"
			+ " FROM \"�b�l�P�V�d��\" \n"
			;
		static string CM17_cmdQuery_ORDER
			= " ORDER BY ���X���b�c,���X���b�c \n"
			;
		static string CM17_key1 = "GKv6R8ei5HofV3yx";			//16����(128�ޯ�)
		static string CM17_keyIV_w = "E93fgcYu";				// 8����(64�ޯ�)
		[WebMethod]
//		public String[] EX_CM17GET(string[] sUser)
		public String[] EX_CM17GET(string[] sUser,string[] s���, string sZenUpdate)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�P�V�}�X�^�擾�����J�n");
			string CM17_cmdQuery = CM17_cmdQuery_SELECT;

			// �����X�V�����̎擾
			CM17_cmdQuery += "WHERE �X�V���� >= "+ sZenUpdate +" \n";

			CM17_cmdQuery += CM17_cmdQuery_ORDER;
			string[] sRetWeb = EX_GetData(sUser, CM17_cmdQuery, CM17_key1, CM17_keyIV_w);

//			// �����X�V�����̍X�V
//			if(sRetWeb[0].Length == 4 && sRetDate[2].Length > 1){
//				logWriter(sUser, INF, "���񓯊�����["+sRetDate[2]+"]");
//				string[] sRet = EX_XM01_SetUpdDate(sUser,"echo","CM17",sRetDate[2]);
//			}
			if(sRetWeb[0].Equals("���M�Ώۃf�[�^����")){
				sRetWeb[0] = "�Y������";
			}
			return sRetWeb;
		}

		/*********************************************************************
		 * �G�R�[�����b�l�P�W�}�X�^�擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM18_cmdQuery 
			= "SELECT \n"
			+ " �o�^��,\"�V�[�P���X�m�n\",�D�揇,�Ǘ��ҋ敪,�\��,�ڍד��e \n"
			+ ",�{�^����,�A�h���X,�X���b�c,����b�c,\"���b�Z�[�W\" \n"
			+ ",�\�����ԊJ�n,�\�����ԏI��,�\���e�f \n"
			+ ",�폜�e�f,�o�^����,�o�^�o�f,�o�^��,�X�V����,�X�V�o�f,�X�V�� \n"
			+ " FROM \"�b�l�P�W���m�点\" \n"
			+ " ORDER BY �o�^��,\"�V�[�P���X�m�n\" \n"
			;
		static string CM18_key1 = "0e1kZgvPiZMAqSUk";			//16����(128�ޯ�)
		static string CM18_keyIV_w = "jE3Z5sd2";				// 8����(64�ޯ�)

		[WebMethod]
		public String[] EX_CM18GET(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�P�W�}�X�^�擾�����J�n");
			return EX_GetData(sUser, CM18_cmdQuery, CM18_key1, CM18_keyIV_w);
		}

		/*********************************************************************
		 * �G�R�[�����b�l�P�X�}�X�^�擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string CM19_cmdQuery 
			= "SELECT \n"
			+ " �X�֔ԍ�,�Z��,�Z���b�c,�X���b�c \n"
			+ ",�폜�e�f,�o�^����,�o�^�o�f,�o�^��,�X�V����,�X�V�o�f,�X�V�� \n"
			+ " FROM \"�b�l�P�X�X�֏Z��\" \n"
			+ " ORDER BY �X�֔ԍ�,�Z��,�Z���b�c,�X���b�c \n"
			;
		static string CM19_key1 = "YZV4EfVwT63CwpRp";			//16����(128�ޯ�)
		static string CM19_keyIV_w = "b7JFvHaS";				// 8����(64�ޯ�)

		[WebMethod]
		public String[] EX_CM19GET(string[] sUser)
		{
			logWriter(sUser, INF, "�G�R�[�����b�l�P�X�}�X�^�擾�����J�n");
			return EX_GetData(sUser, CM19_cmdQuery, CM19_key1, CM19_keyIV_w);
		}

		/*********************************************************************
		 * �G�R�[�����}�X�^�擾���ʏ���
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		private String[] EX_GetData(string[] sUser, string cmdQuery, string key1, string keyIV_w)
		{
			// �f�[�^��
			string[] sPreEncData;
			int iCnt;

			sPreEncData = null;
			ArrayList arList = new ArrayList();
			ArrayList arList2 = new ArrayList();
			ArrayList sList2 = new ArrayList();
			string sEncChar = "";

			string[] sRet = new string[2];

			// �c�a�ڑ�
			OracleConnection conn2 = null;
			conn2 = connect2(sSvUser);

			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
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
					// �f�[�^�Ȃ��̃��O
					logWriter(sUser, INF," ���M�Ώۃf�[�^�͂���܂���ł����B");
					sRet[0] = "���M�Ώۃf�[�^����";
					return sRet;
				}
				else
				{
					//�f�[�^����B
					iCnt = 1;
					
					logWriter(sUser, INF," �f�[�^�ϊ��J�n");

					IEnumerator enumList = arList.GetEnumerator();
					
					// 3DES ����KEY�̒�`
					byte[] DesIV = Encoding.UTF8.GetBytes(keyIV_w);
					string DesKey3 = key1;
					byte[] DesKey = Encoding.UTF8.GetBytes(DesKey3);
					//
					iCnt = 0;
					 
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();

						// ������� byte �z��ɕϊ����܂�
						byte[] source = Encoding.UTF8.GetBytes(enumList.Current.ToString());
						
						// Triple DES �̃T�[�r�X �v���o�C�_�𐶐����܂�
						TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

						// 
						// ���o�͗p�̃X�g���[���𐶐����܂�
						MemoryStream ms = new MemoryStream();
						CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor( DesKey, DesIV ),
							CryptoStreamMode.Write);

						// �X�g���[���ɈÍ�������f�[�^���������݂܂�
						cs.Write(source, 0, source.Length);
						cs.Close();
							
						// �Í������ꂽ�f�[�^�� byte �z��Ŏ擾���܂�
						byte[] b�Í��f�[�^ = ms.ToArray();
						ms.Close();
						
						// byte �z��𕶎���ɕϊ�����ARRAY�ɃZ�b�g���܂�
						sEncChar = "";
						for(int iCharCnt = 0; iCharCnt < b�Í��f�[�^.Length; iCharCnt++)
						{
							sEncChar += b�Í��f�[�^[iCharCnt].ToString("X2");
						}	
						arList2.Add(sEncChar);
						iCnt++;
					}
					//�Í���Array ����
					sPreEncData = new string[arList2.Count];

					sPreEncData = (string[])arList2.ToArray(typeof(string));
					//�ʏ�f�[�^��Array ����
					sRet = new string[arList.Count + 1];
					IEnumerator enumList4 = arList2.GetEnumerator();
					sRet[0] = "����I��";
					int iCnt3 = 1;
					while (enumList4.MoveNext())
					{
						sRet[iCnt3] = enumList4.Current.ToString();
						iCnt3++;
					}
					logWriter(sUser, INF, iCnt3 - 1 + " ���̃f�[�^�擾�������I�����܂����B");
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				logWriter(sUser, ERR, sRet[0]);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �G�R�[�����t�@�C���擾���ʏ���
		 * �����F
		 * �ߒl�F�X�e�[�^�X
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
		 * �G�R�[�����t�@�C���擾���ʏ���(String)
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		private String[] EX_GetFileS(string[] sUser, string sFile)
		{

			string [] sResult = new string[1];
			string sFileData = string.Empty;
			string sErrMsg = string.Empty;
			// �_���v�t�@�C���p�X
			string sDumpFile = Path.Combine(sDumpFolder, sFile + ".enc");
//			// �t���O�t�@�C���p�X
//			string sFlagFile = Path.Combine(sDumpFolder, sFile + ".sts");

//			// �t���O�t�@�C���̑��݃`�F�b�N
//			if (! File.Exists(sFlagFile))
//			{
//				sErrMsg = string.Format("�t���O�t�@�C��������܂���B[{0}]", sFlagFile);
//				logWriter(sUser, ERR, sErrMsg);
//				return new string [] {sErrMsg};
//			}

			// �t�@�C���̑��݃`�F�b�N
			if (! File.Exists(sDumpFile))
			{
				sErrMsg = string.Format("�f�[�^�t�@�C��������܂���B[{0}]", sDumpFile);
				logWriter(sUser, ERR, sErrMsg);
				return new string [] {sErrMsg};
			}

			try 
			{
				// �t�@�C����ǂݍ���
				using (StreamReader sr = new StreamReader(sDumpFile, Encoding.GetEncoding("shift_jis")))
				{
					sFileData = sr.ReadToEnd();
				}
			} 
			catch (Exception ex) 
			{
				sErrMsg = string.Format("�t�@�C���̓ǂݎ��Ɏ��s���܂����B[{0}] - {1}", sDumpFile, ex.Message);
				logWriter(sUser, ERR, sErrMsg);
				return new string [] {sErrMsg};
			}

//			// �t���O�t�@�C���폜
//			if (File.Exists(sFlagFile)) File.Delete(sFlagFile);

			// �߂�l�̏���
			sResult = new string[2];
			sResult[0] = "����I��";
			sResult[1] = sFileData;

			return sResult;
			
		}

		/*********************************************************************
		 * �G�R�[�����e�[�u�������X�V�����擾����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] EX_XM01_GetUpdDate(string[] sUser, string sKaiinCD, string sTableKey)
		{
			DateTime dtNow = DateTime.Now;
			string[] sRet = new string[3];

			// �c�a�ڑ�
			OracleConnection conn2 = null;
			conn2 = connect2(sSvUser);

			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			try{
				string cmdQuery = "SELECT �c�a�X�V���� \n"
								+ " FROM �w�l�O�P�X�V���� \n "
								+ " WHERE ����b�c = '"+sKaiinCD+"' \n"
								+ " AND \"�c�a�L�[\" = '"+sTableKey+"' \n"
								+ " AND �폜�e�f = '0' \n"
								;
				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				if(reader.Read())
				{
					sRet[0] = "����I��";
					sRet[1] = reader.GetDecimal(0).ToString();
				}
				else
				{
					sRet[0] = "�Y������";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �G�R�[�����e�[�u�������X�V�����X�V����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] EX_XM01_SetUpdDate(string[] sUser, string[] sKaiinCD, string sTableKey, string sUpdDate, string sUpdPG, string sUpdSya)
		{
			string[] sRet = new string[3];

			// �c�a�ڑ�
			OracleConnection conn2 = null;
			conn2 = connect2(sSvUser);

			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();
			try{
				int iCnt = 1;
				while (iCnt < sKaiinCD.Length)
				{
					string cmdQuery = "UPDATE �w�l�O�P�X�V���� SET \n "
								+ " �c�a�X�V���� = "+sUpdDate+"\n"
								+ ", �X�V���� =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
								+ ",�X�V�o�f  = '" + sUpdPG +"'"
								+ ",�X�V��    = '" + sUpdSya +"' \n"
								+ " WHERE ����b�c = '"+sKaiinCD[iCnt]+"' \n"
								+ " AND \"�c�a�L�[\" = '"+sTableKey+"' \n"
								+ " AND �폜�e�f = '0' \n"
								;
					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					if(iUpdRow == 0)
					{
						cmdQuery = "INSERT INTO �w�l�O�P�X�V����( \n "
								+ " ����b�c,�c�a�L�[,�c�a�X�V����,�c�a�X�V���t \n"
								+ ",�폜�e�f \n"
								+ ",�o�^����, �o�^�o�f, �o�^�� \n"
								+ ",�X�V����, �X�V�o�f, �X�V�� \n"
								+ ")VALUES( \n"
								+ " '"+sKaiinCD[iCnt]+"' \n"
								+ ",'"+sTableKey+"' \n"
								+ ", "+sUpdDate+" \n"
								+ ", 0 \n"
								+ ", '0' \n"
								+ ", TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sUpdPG +"','" + sUpdSya +"' \n"	//�o�^�����`
								+ ", TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sUpdPG +"','" + sUpdSya +"' \n"   //�X�V�����`		
								+ " ) \n"
								;
						iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					}
					if(iUpdRow == 1)
					{
						sRet[0] = "����I��";
					}
					else
					{
						sRet[0] = "�o�^�G���[";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �G�R�[�����r�l�O�P�}�X�^�X�V����
		 * �����F
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		static string SM01_key1 = "rTqpnRaWRNjrl6e8";			//16����(128�ޯ�)
		static string SM01_keyIV_w = "BN3bT2q3";				// 8����(64�ޯ�)

		string sSM01_�e�[�u���� = "\"�r�l�O�P�ב��l\"";
		string sSM01_���ږ� 
			= " ����b�c,����b�c,�ב��l�b�c \n"
			+ ",���Ӑ�b�c,���Ӑ敔�ۂb�c \n"
			+ ",�d�b�ԍ��P,�d�b�ԍ��Q,�d�b�ԍ��R,�e�`�w�ԍ��P,�e�`�w�ԍ��Q,�e�`�w�ԍ��R \n"
			+ ",�Z���P,�Z���Q,�Z���R,���O�P,���O�Q,���O�R \n"
			+ ",�X�֔ԍ�,�J�i����,�ː�,�d��,�׎D�敪,\"���[���A�h���X\" \n"
			+ ",�폜�e�f,�o�^����,�o�^�o�f,�o�^��,�X�V����,�X�V�o�f,�X�V�� \n"
			;

		[WebMethod]
		public string[] EX_SM01SET(string[] sUser, string[] sGetData)
		{
			logWriter(sUser, INF, "�G�R�[�����r�l�O�P�}�X�^�X�V�����J�n");

			string[] sRet = {"","",""};
			int in_cnt = 0;

			//---- �Í����t�@�C���̕���
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

				// Trippe DES �̃T�[�r�X �v���o�C�_�𐶐����܂�
				TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

				// ���o�͗p�̃X�g���[���𐶐����܂�(����)
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor( DesKey, DesIV),
				CryptoStreamMode.Write);

				// �X�g���[���ɈÍ������ꂽ�f�[�^���������݂܂�
				cs.Write(bText, 0, bText.Length);
				cs.Close();

				// ���������ꂽ�f�[�^�� byte �z��Ŏ擾���܂�
				byte[] destination = ms.ToArray();
				ms.Close();

				// byte �z��𕶎���ɕϊ�����ARRAY�ɕۑ�����
				aData.Add(Encoding.UTF8.GetString(destination));
				// ���������ꂽ1�s�𗭂ߍ��ށ@
			}

			//----�@�c�a����
			OracleConnection conn2 = null;

			// �c�a�ڑ�
			conn2 = connect2(sSvUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
	
			OracleTransaction tran;
			tran = conn2.BeginTransaction();
		
			try
			{
//				// �e�[�u���S���폜
//				string cmdQuery = "DELETE FROM " + s�e�[�u����;
//				int iDelRtn = CmdUpdate(sMyUser, conn2, cmdQuery);

				string[] s���ږ� = sSM01_���ږ�.Split(',');
				StringBuilder sbBuffer;
				IEnumerator enumList = aData.GetEnumerator();
				int i�X�V���� = 0;
				int i�o�^���� = 0;
				while(enumList.MoveNext())
				{
					string sStr = enumList.Current.ToString();
					string[] s���� = sStr.Split(',');
					sbBuffer = new StringBuilder(4096);
					sbBuffer.Append("UPDATE "+ sSM01_�e�[�u���� + " SET \n");
					for(int iCnt = 3; iCnt < s���ږ�.Length; iCnt++){
						if(iCnt == 3){
							sbBuffer.Append(" "+s���ږ�[iCnt]+" = "+ s����[iCnt] + " \n");
						}else{
							sbBuffer.Append(","+s���ږ�[iCnt]+" = "+ s����[iCnt] + " \n");
						}
					}
					sbBuffer.Append("WHERE ����b�c = " + s����[0] + " \n");
					sbBuffer.Append("AND ����b�c = " + s����[1] + " \n");
					sbBuffer.Append("AND �ב��l�b�c = " + s����[2] + " \n");
					int iUpdRow = CmdUpdate(sUser, conn2, sbBuffer);
					if(iUpdRow == 0){
						sbBuffer = new StringBuilder(4096);
						sbBuffer.Append("INSERT INTO "+ sSM01_�e�[�u���� + "( \n");
						sbBuffer.Append(sSM01_���ږ�);
						sbBuffer.Append(")VALUES( \n");
						sbBuffer.Append(sStr);
						sbBuffer.Append(") \n");

						iUpdRow = CmdUpdate(sUser, conn2, sbBuffer);
						i�o�^���� += iUpdRow;
					}else{
						i�X�V���� += iUpdRow;
					}
				}
				tran.Commit();
				sRet[0] = "����I��";
				sRet[1] = i�o�^����.ToString();
				sRet[2] = i�X�V����.ToString();
				logWriter(sUser, INF, " �o�^�����F"+sRet[1]);
				logWriter(sUser, INF, " �X�V�����F"+sRet[2]);
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
