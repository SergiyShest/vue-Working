/*
 Version information:
    $Revision: 35447 $
    $Author: Shestakov $
    $Date: 2015-04-29 12:28:33 +0400 (Ср, 29 апр 2015) $
*/

using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Data.OleDb;

using UBP.Common;


namespace UBP.DataExport
{
	public static class DBF
	{
		/// <summary>
		/// Хранит информацию о поле дбф-таблицы: имя, тип данных.
		/// </summary>
		public class dbf_field
		{
			public string Name;
			public string DataType;
			public dbf_field(string name, string data_type)
			{
				this.Name = name;
				this.DataType = data_type;
			}
			public OleDbType ParamType
			{
				get
				{
					OleDbType t = OleDbType.VarChar;
					switch (DataType.ToUpper().Substring(0, 1))
					{
						case "C":
							t = OleDbType.VarChar;
							break;
						case "L":
							t = OleDbType.Boolean;
							break;
						case "I":
							t = OleDbType.Integer;
							break;
						case "N":
							t = OleDbType.Numeric;
							break;
						case "D":
							t = OleDbType.Date;
							break;
					}
					return t;
				}
			}
		}
		/// <summary>
		/// Выгружает данные в дбф-таблицу.Создаёт файл по заданому пути и грузит туда данные из дататейбл по заданным столбцам
		/// </summary>
		/// <param name="tbl">DataTable с данными для выгрузки</param>
		/// <param name="full_file_name">полный путь к создаваемому дбфнику - путь + имя файла</param>
		/// <param name="fields">список полей дбфника с их типами данных. задаёт создаваемые в дбфнике столбцы и вгружаемые из дататейбл данные.</param>
		public static void save_dbf(DataTable tbl, string full_file_name, dbf_field[] fields)
		{
			OleDbConnection con_dbf = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" +
				 System.IO.Path.GetDirectoryName(full_file_name) +
				 ";Password=;Collating Sequence=MACHINE;Extended Properties=dBASE III");
			con_dbf.Open();
			string file_name = System.IO.Path.GetFileNameWithoutExtension(full_file_name).Replace(' ', '_');
			using (OleDbCommand cmd_create_tbl = new OleDbCommand("create table " +
				 file_name + "(", con_dbf))
			{
				foreach (dbf_field f in fields)
				{ cmd_create_tbl.CommandText += f.Name + " " + f.DataType + ","; }
				cmd_create_tbl.CommandText = cmd_create_tbl.CommandText.Remove(cmd_create_tbl.CommandText.Length - 1);
				cmd_create_tbl.CommandText += ");";
				try
				{
					cmd_create_tbl.ExecuteNonQuery();
				}
				catch (Exception ex) { REnvironment.Current.ShowExceptionMessage(ex.ToString()); return; }
			}
			using (OleDbCommand cmd_ins = new OleDbCommand("insert into " +
				 file_name + " (", con_dbf))
			{
				string vals = string.Empty;
				foreach (dbf_field f in fields)
				{
					cmd_ins.CommandText += f.Name + ",";
					vals += "?,";
					cmd_ins.Parameters.Add(f.Name, f.ParamType);
				}
				cmd_ins.CommandText = cmd_ins.CommandText.Remove(cmd_ins.CommandText.Length - 1);
				cmd_ins.CommandText += ") values (" + vals.Remove(vals.Length - 1) + ");";

				foreach (DataRow r in tbl.Rows)
				{
					foreach (dbf_field f in fields)
					{
						cmd_ins.Parameters[f.Name].Value = r[f.Name];
					}

					try
					{
						cmd_ins.ExecuteNonQuery();
					}
                    catch (Exception ex) { REnvironment.Current.ShowExceptionMessage(ex.ToString()); return; }
				}
			}
			con_dbf.Close();
			con_dbf.Dispose();
			GC.Collect();
		}

		public static void DataTableSaveToDBF(DataTable dataTable, DataTable discription, string folder, string tblname)
		{
			// Создаю таблицу
			#region  header
			tblname = System.Text.RegularExpressions.Regex.Replace(tblname, ".DBF$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

			System.IO.File.Delete(folder + "\\" + tblname + ".DBF");
			System.IO.FileStream FS = new System.IO.FileStream(folder + "\\" + tblname + ".DBF", System.IO.FileMode.Create);
			// Формат dBASE III 2.0
			byte[] buffer = new byte[] { 0x03, 0x63, 0x04, 0x04 }; // Заголовок  4 байта
			FS.Write(buffer, 0, buffer.Length);
			buffer = new byte[]{
                       (byte)(((dataTable.Rows.Count % 0x1000000) % 0x10000) % 0x100),
                       (byte)(((dataTable.Rows.Count % 0x1000000) % 0x10000) / 0x100),
                       (byte)(( dataTable.Rows.Count % 0x1000000) / 0x10000),
                       (byte)(  dataTable.Rows.Count / 0x1000000)
                      }; // Word32 -> кол-во строк 5-8 байты
			FS.Write(buffer, 0, buffer.Length);
			int i = (dataTable.Columns.Count + 1) * 32 + 1; // Изврат
			buffer = new byte[]{
                       (byte)( i % 0x100),
                       (byte)( i / 0x100)
                      }; // Word16 -> кол-во колонок с извратом 9-10 байты
			FS.Write(buffer, 0, buffer.Length);
			string[] FieldName = new string[dataTable.Columns.Count]; // Массив названий полей
			string[] FieldType = new string[dataTable.Columns.Count]; // Массив типов полей
			byte[] FieldSize = new byte[dataTable.Columns.Count]; // Массив размеров полей
			byte[] FieldDigs = new byte[dataTable.Columns.Count]; // Массив размеров дробной части
			int s = 1; // Считаю длину заголовка

			foreach (DataColumn C in dataTable.Columns)
			{
				string l = C.ColumnName.ToUpper(); // Имя колонки
				while (l.Length < 10)
				{
					l = l + (char)0;
				} // Подгоняю по размеру (10 байт)
				FieldName[C.Ordinal] = l.Substring(0, 10) + (char)0; // Результат
				FieldType[C.Ordinal] = "C";
				FieldSize[C.Ordinal] = 50;
				FieldDigs[C.Ordinal] = 0;
				byte numeric = 28;
				byte scale = 0;
				if (C.DataType != typeof(string))

					try
					{
						if (discription != null)
						{
							var numericV = discription.Rows[C.Ordinal]["NumericPrecision"];

							if (numericV != DBNull.Value)
							{
								numeric = Convert.ToByte(numericV);
							}
							var scaleV = discription.Rows[C.Ordinal]["NumericScale"];
							if (scaleV != DBNull.Value)
							{
								scale = Convert.ToByte(scaleV);
							}
						}
					}   //Convert.ToByte(C.ExtendedProperties["NumericScale"]);
					catch
					{

					}
				if (scale == 127)//такое возможно только если значения не определены и нужно поставить по умолчанию
				{
					SetDefSize(ref numeric, ref scale, C);
				}
				switch (C.DataType.ToString())
				{
					case "System.String":
						{

							Byte b;

							if (C.MaxLength <= 254)
							{
								b = Convert.ToByte(C.MaxLength);
							}
							else
							{
								//throw new Exception("Невозможно создать Корректный DBF Файл так как максимальная длина строки не может превышать 254 байт у колонки "+C.ColumnName +" длина="+C.MaxLength);
								b = Convert.ToByte(254);
							}
							FieldSize[C.Ordinal] = b;




							if (FieldSize[C.Ordinal] == 0)
								FieldSize[C.Ordinal] = 1;
							break;
						}
					case "System.Boolean": { FieldType[C.Ordinal] = "L"; FieldSize[C.Ordinal] = 1; break; }
					case "System.Byte": { FieldType[C.Ordinal] = "N"; FieldSize[C.Ordinal] = 1; break; }
					case "System.DateTime": { FieldType[C.Ordinal] = "D"; FieldSize[C.Ordinal] = 8; break; }
					case "System.Decimal": { FieldType[C.Ordinal] = "N"; FieldSize[C.Ordinal] = numeric; FieldDigs[C.Ordinal] = scale; break; }
					case "System.Double": { FieldType[C.Ordinal] = "F"; FieldSize[C.Ordinal] = numeric; FieldDigs[C.Ordinal] = scale; break; }
					case "System.Int16": { FieldType[C.Ordinal] = "N"; FieldSize[C.Ordinal] = numeric; break; }
					case "System.Int32": { FieldType[C.Ordinal] = "N"; FieldSize[C.Ordinal] = numeric; break; }
					case "System.Int64": { FieldType[C.Ordinal] = "N"; FieldSize[C.Ordinal] = numeric; break; }
					case "System.SByte": { FieldType[C.Ordinal] = "N"; FieldSize[C.Ordinal] = numeric; break; }
					case "System.Single": { FieldType[C.Ordinal] = "F"; FieldSize[C.Ordinal] = numeric; FieldDigs[C.Ordinal] = scale; break; }
					case "System.UInt16": { FieldType[C.Ordinal] = "N"; FieldSize[C.Ordinal] = numeric; break; }
					case "System.UInt32": { FieldType[C.Ordinal] = "N"; FieldSize[C.Ordinal] = numeric; break; }
					case "System.UInt64": { FieldType[C.Ordinal] = "N"; FieldSize[C.Ordinal] = numeric; break; }
				}
				s = s + FieldSize[C.Ordinal];
			}
			buffer = new byte[]{
                       (byte)(s % 0x100), 
                       (byte)(s / 0x100)
                      }; // Пишу длину заголовка 11-12 байты
			FS.Write(buffer, 0, buffer.Length);
			for (int j = 0; j < 20; j++) { FS.WriteByte(0x00); } // Пишу всякий хлам - 20 байт, 
			//  итого: 32 байта - базовый заголовок DBF
			// Заполняю заголовок
			foreach (DataColumn C in dataTable.Columns)
			{
				buffer = System.Text.Encoding.Default.GetBytes(FieldName[C.Ordinal]); // Название поля
				FS.Write(buffer, 0, buffer.Length);
				buffer = new byte[]{
                        System.Text.Encoding.ASCII.GetBytes(FieldType[C.Ordinal])[0],
                        0x00, 
                        0x00,
                        0x00, 
                        0x00
                       }; // Размер
				FS.Write(buffer, 0, buffer.Length);
				buffer = new byte[]{
                        FieldSize[C.Ordinal],
                        FieldDigs[C.Ordinal]
                       }; // Размерность
				FS.Write(buffer, 0, buffer.Length);
				buffer = new byte[]{0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00}; // 14 нулей
				FS.Write(buffer, 0, buffer.Length);
			}
			FS.WriteByte(0x0D); // Конец описания таблицы

			#endregion

			System.Globalization.DateTimeFormatInfo dfi = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
			System.Globalization.NumberFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
			string Spaces = "";
			while (Spaces.Length < 255) Spaces = Spaces + " ";
			foreach (DataRow R in dataTable.Rows)
			{
				FS.WriteByte(0x20); // Пишу данные
				foreach (DataColumn C in dataTable.Columns)
				{
					string l = R[C].ToString();
					if (l != "") // Проверка на NULL
					{
						switch (FieldType[C.Ordinal])
						{
							case "L":
								{
									l = bool.Parse(l).ToString();
									break;
								}
							case "N":
								{
									l = decimal.Parse(l).ToString(nfi);
									break;
								}
							case "F":
								{
									l = float.Parse(l).ToString(nfi);
									break;
								}
							case "D":
								{
									l = DateTime.Parse(l).ToString("yyyyMMdd", dfi);
									break;
								}
							default: l = l.Trim() + Spaces; break;
						}
					}
					else
					{
						if (FieldType[C.Ordinal] == "C"
						 || FieldType[C.Ordinal] == "D")
							l = Spaces;
					}
					while (l.Length < FieldSize[C.Ordinal]) { l = l + (char)0x00; }
					l = l.Substring(0, FieldSize[C.Ordinal]); // Корректирую размер
					buffer = System.Text.Encoding.GetEncoding(866).GetBytes(l); // Записываю в кодировке (MS-DOS Russian)
					FS.Write(buffer, 0, buffer.Length);
				}
			}
			FS.WriteByte(0x1A); // Конец данных
			FS.Close();
		}


		internal static void SetDefSize(ref  byte numeric, ref byte scale, DataColumn C)
		{
			switch (C.DataType.ToString())
			{
				case "System.Byte":
					{

						numeric = 1;
						break;
					}
				case "System.DateTime":
					{

						numeric = 8;
						break;
					}
				case "System.Decimal":
					{

						numeric = 38;
						scale = 5;
						break;
					}
				case "System.Double":
					{

						numeric = 38;
						scale = 5;
						break;
					}
				case "System.Int16":
					{

						numeric = 6;
						break;
					}


				case "System.Int32":
					{
						numeric = 11;
						break;
					}
				case "System.Int64":
					{

						numeric = 21;
						break;
					}
				case "System.SByte":
					{

						numeric = 6;
						break;
					}
				case "System.Single":
					{

						numeric = 38;
						scale = 5;
						break;
					}
				case "System.UInt16":
					{

						numeric = 6;
						break;
					}
				case "System.UInt32":
					{

						numeric = 11;
						break;
					}
				case "System.UInt64":
					{

						numeric = 21;
						break;
					}
			}
		}
	}

	/// <summary>
	/// класс для работы с дбф 
	/// </summary>
	public class DBFx
	{

		private long m_RowCount;
		private FileStream _fileStreem;
		private string[] _fieldType;
		private byte[] _fieldSize;
		private byte[] _fieldDigs;
		private DataTable _etalonDataTable;
		private Encoding _encoding;
		private List<Int32> _usingColumnList;
		private string[] _data;

		public string[] Data
		{
			get { return _data; }

		}
		private System.Globalization.DateTimeFormatInfo dfi =
			 new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;

		private System.Globalization.NumberFormatInfo nfi =
			 new System.Globalization.CultureInfo("en-US", false).NumberFormat;

		//  private string Spaces1 = string.Empty;



		public DBFx(DataTable dataTable, DataTable shemaTable, List<string> usingColumnList,
			 string fileName)
		{
			//    while (Spaces.Length < 255) Spaces = Spaces + " ";

			_etalonDataTable = dataTable;
			_encoding = Encoding.GetEncoding(866);
			string folder = Path.GetDirectoryName(fileName);
			string tblname = Path.GetFileNameWithoutExtension(fileName);
		    string ext = Path.GetExtension(fileName);
		    if (string.IsNullOrWhiteSpace(ext))
		    {
		        ext = "DBF"; }
			_usingColumnList = new List<int>(usingColumnList.Count);
			_data = new string[dataTable.Columns.Count];




            File.Delete(fileName);
            _fileStreem = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 500000);

			#region prepare

			// Формат dBASE III 2.0
			byte[] buffer = new byte[] { 0x03, 0x63, 0x04, 0x04 }; // Заголовок  4 байта
			_fileStreem.Write(buffer, 0, buffer.Length);

			buffer = new byte[]
                     {
                         (byte) (((dataTable.Rows.Count%0x1000000)%0x10000)%0x100),
                         (byte) (((dataTable.Rows.Count%0x1000000)%0x10000)/0x100),
                         (byte) ((dataTable.Rows.Count%0x1000000)/0x10000),
                         (byte) (dataTable.Rows.Count/0x1000000)
                     }; // Word32 -> кол-во строк 5-8 байты
			_fileStreem.Write(buffer, 0, buffer.Length);

			int i = (usingColumnList.Count + 1) * 32 + 1; // Изврат
			buffer = new byte[]
                     {
                         (byte) (i%0x100),
                         (byte) (i/0x100)
                     }; // Word16 -> кол-во колонок с извратом 9-10 байты
			_fileStreem.Write(buffer, 0, buffer.Length);
			string[] FieldName = new string[usingColumnList.Count]; // Массив названий полей
			_fieldType = new string[usingColumnList.Count]; // Массив типов полей
			_fieldSize = new byte[usingColumnList.Count]; // Массив размеров полей
			_fieldDigs = new byte[usingColumnList.Count];// Массив размеров дробной части
			int s = 1; // Считаю длину заголовка

			#endregion

			#region  получение заголовка

			int columnNum = 0;
			foreach (string columnN in usingColumnList)
			{
				
 int columnNumOrig = 0;
                DataColumn column = findColumn(columnN,out columnNumOrig );

                if(column!=null)
				{_usingColumnList.Add(column.Ordinal);}
                else
                {
                   REnvironment.Current.Protocol.PutExceptionMessage(new Exception("В запросе нет колонки с именем "+columnN+" но она есть в отчете" ));
                continue;
                }

				string columnName = columnN;

				while (columnName.Length < 10)
				{
					columnName = columnName + (char)0;
				} // Подгоняю по размеру (10 байт)
				FieldName[columnNum] = columnName.Substring(0, 10) + (char)0; // Результат
				_fieldType[columnNum] = "C";
				_fieldSize[columnNum] = 50;
				_fieldDigs[columnNum] = 0;
				byte numeric = 28;
				byte scale = 0;

				if (column.DataType != typeof(string))
				{
					try
					{
						if (shemaTable != null)
						{
							var numericV = shemaTable.Rows[columnNum]["NumericPrecision"];

							if (numericV != DBNull.Value)
							{
								numeric = Convert.ToByte(numericV);
							}
							var scaleV = shemaTable.Rows[columnNum]["NumericScale"];
							if (scaleV != DBNull.Value)
							{
								scale = Convert.ToByte(scaleV);
							}
						}
					} //Convert.ToByte(C.ExtendedProperties["NumericScale"]);
					catch
					{

					}
				}

				if (scale == 127) //такое возможно только если значения не определены и нужно поставить по умолчанию
				{
					DBF.SetDefSize(ref numeric, ref scale, column);
				}

				switch (column.DataType.ToString())
				{

					case "System.String":
						{

							Byte b;

							if (column.MaxLength <= 254)
							{
								b = Convert.ToByte(column.MaxLength);
							}
							else
							{
								REnvironment.Current.Protocol.PutExceptionMessage(new Exception(
									 "Невозможно создать Корректный DBF Файл так как максимальная длина строки не может превышать 254 байт у колонки " +
									 column.ColumnName + " длина=" + column.MaxLength));
								b = Convert.ToByte(254);
							}
							_fieldSize[columnNum] = b;

							if (_fieldSize[columnNum] == 0)
								_fieldSize[columnNum] = 1;
							break;
						}

					case "System.Boolean":
						{
							_fieldType[columnNum] = "L";
							_fieldSize[columnNum] = 1;
							break;
						}
					case "System.Byte":
						{
							_fieldType[columnNum] = "N";
							_fieldSize[columnNum] = 1;
							break;
						}
					case "System.DateTime":
						{
							_fieldType[columnNum] = "D";
							_fieldSize[columnNum] = 8;
							break;
						}
					case "System.Decimal":
						{
							_fieldType[columnNum] = "N";
							_fieldSize[columnNum] = numeric;
							_fieldDigs[columnNum] = scale;
							break;
						}
					case "System.Double":
						{

						    if (numeric + scale > 30)
						    {
						        _fieldType[columnNum] = "F";
						    }
						    else
						    {
						        	_fieldType[columnNum] = "N";
						    }
							_fieldSize[columnNum] = numeric;
							_fieldDigs[columnNum] = scale;
							break;
						}
					case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.SByte":
						{
							_fieldType[columnNum] = "N";
							_fieldSize[columnNum] = numeric;
							break;
						}
					

					case "System.Single":
						{
							_fieldType[columnNum] = "F";
							_fieldSize[columnNum] = numeric;
							_fieldDigs[columnNum] = scale;
							break;
						}
					case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
						{
							_fieldType[columnNum] = "N";
							_fieldSize[columnNum] = numeric;
							break;
						}

				}
				s = s + _fieldSize[columnNum];

				columnNum++;
              //  REnvironment.Current.Protocol.PutDebugMessage("Сформирована колонка "+columnN+" "+column.DataType+" "+numeric+ Environment.NewLine);
			}

			#endregion

			#region заполнение заголовка

			buffer = new byte[]
                     {
                         (byte) (s%0x100),
                         (byte) (s/0x100)
                     }; // Пишу длину заголовка 11-12 байты
			_fileStreem.Write(buffer, 0, buffer.Length);
			for (int j = 0; j < 20; j++)
			{
				_fileStreem.WriteByte(0x00);
			} // Пишу всякий хлам - 20 байт, 
			//  итого: 32 байта - базовый заголовок DBF
			// Заполняю заголовок


			for (columnNum = 0; columnNum < _usingColumnList.Count; columnNum++)
			{

				buffer = _encoding.GetBytes(FieldName[columnNum]); // Название поля
				_fileStreem.Write(buffer, 0, buffer.Length);
				buffer = new byte[]
                         {
                             _encoding.GetBytes(_fieldType[columnNum])[0],
                             0x00,
                             0x00,
                             0x00,
                             0x00
                         }; // Размер
				_fileStreem.Write(buffer, 0, buffer.Length);
				buffer = new byte[]
                         {
                             _fieldSize[columnNum],
                             _fieldDigs[columnNum]
                         }; // Размерность
				_fileStreem.Write(buffer, 0, buffer.Length);
				buffer = new byte[]
                         {
                             0x00, 0x00, 0x00, 0x00, 0x00,
                             0x00, 0x00, 0x00, 0x00, 0x00,
                             0x00, 0x00, 0x00, 0x00
                         }; // 14 нулей
				_fileStreem.Write(buffer, 0, buffer.Length);

			}
			_fileStreem.WriteByte(0x0D); // Конец описания таблицы

			#endregion

		}

private DataColumn findColumn(string columnName ,out int columnNumOrig)
		{
		    columnNumOrig = 0;
			foreach (DataColumn column in _etalonDataTable.Columns)
			{
			    
				if (String.Compare(column.ColumnName, columnName, true) == 0)
				{
					return column;
				}
                columnNumOrig++;
			}
			return null;
		}

		internal void WriteDbfRow(IRDataReader reader)
		{
			_fileStreem.WriteByte(0x20); // Пишу данные

			int i = 0;

			foreach (int columnn in _usingColumnList)
			{
				string l = String.Empty;
				object obj = reader[columnn];

				/*if (obj != DBNull.Value)
				{
					 l = obj.ToString();
				}*/

				if (obj != DBNull.Value) // Проверка на NULL
				{
					switch (_fieldType[i])
					{
						case "L":
							{
								l = System.Convert.ToBoolean(obj).ToString();
								break;
							}
						case "N":
							{
								l = System.Convert.ToDecimal(obj).ToString(nfi);
								break;
							}
						case "F":
							{
                                                          if (_fieldDigs[i] == 0)

							    {
							        l = System.Convert.ToSingle(obj).ToString(nfi);
							    }
							    else
							    {
							        l = System.Convert.ToDouble(obj).ToString(nfi);
							    }
								break;
							}
						case "D":
							{
								l = System.Convert.ToDateTime(obj).ToString("yyyyMMdd", dfi);
								break;
							}
						default:
							l = System.Convert.ToString(obj);
							break;
					}
				}

				if (l.Length < _fieldSize[i])
				{
					l = l + new String(' ', _fieldSize[i] - l.Length);
				}
				else if (l.Length > _fieldSize[i])
				{
					l = l.Substring(0, _fieldSize[i]);
				}

				byte[] buffer = _encoding.GetBytes(l); // Записываю в кодировке 
				_fileStreem.Write(buffer, 0, buffer.Length);

				i++;
			}

			this.m_RowCount++;
		}

		public void Close()
		{


			_fileStreem.WriteByte(0x1A); // Конец данных

			byte[] buffer = new byte[]
                     {
                         (byte) (((this.m_RowCount % 0x1000000)%0x10000)%0x100),
                         (byte) (((this.m_RowCount % 0x1000000)%0x10000)/0x100),
                         (byte) ((this.m_RowCount % 0x1000000)/0x10000),
                         (byte) (this.m_RowCount / 0x1000000)
                     }; // Word32 -> кол-во строк 5-8 байты
			_fileStreem.Seek(4, SeekOrigin.Begin);
			_fileStreem.Write(buffer, 0, buffer.Length);

			_fileStreem.Close();
		}
	}

}
