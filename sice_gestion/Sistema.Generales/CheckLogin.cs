﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema.DataModel;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace Sistema.Generales
{
    public class CheckLogin
    {
        public int checkLocal(string usuario, string contrasena)
        {
            try
            {
                //return 1;
               // usuario = "23636b9887b68ebaaaf7b25e1af762e4";
               //contrasena = "e10adc3949ba59abbe56e057f20f883e";
                using (DatabaseContext contexto = new DatabaseContext("MYSQLOCAL"))
                {
                    sice_usuarios usr = (from i in contexto.sice_usuarios where i.correo == usuario && i.contrasena == contrasena select i).FirstOrDefault();
                    

                    if (usr != null)
                    {
                        string consulta =
                        "SELECT c.id_distrito_local FROM sice_casillas C where id_cabecera_local = " + usr.id_municipio + " " +
                        "GROUP BY C.id_distrito_local ";
                        LoginInfo.lista_distritos = contexto.Database.SqlQuery<int>(consulta).ToList();
                        LoginInfo.id_usuario = usr.id;
                        LoginInfo.nombre = usr.nombre;
                        LoginInfo.apellido = usr.apellido;
                        LoginInfo.id_municipio = usr.id_municipio;
                        LoginInfo.privilegios = usr.privilegios;
                        LoginInfo.id_cabecera_local = usr.id_municipio;
                        LoginInfo.nombre_formal = usr.nombre_formal;
                        LoginInfo.grupo_trabajo = (int)usr.grupo_trabajo;
                        return 1;
                    }

                    return 0;
                }


            }
            catch (Exception ex)
            {
                //string innerEx = ex.InnerException.Message;
                //if (innerEx == "Unable to connect to any of the specified MySQL hosts.")
                //{
                //    return 2;
                //}
                //else
                //{
                //    return 3;
                //}
                return 4;


            }

        }

        public int checkServer(string usuario, string contrasena)
        {
            try
            {
                //return 1;
                // usuario = "23636b9887b68ebaaaf7b25e1af762e4";
                //contrasena = "e10adc3949ba59abbe56e057f20f883e";
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    sice_usuarios usr = (from i in contexto.sice_usuarios where i.correo == usuario && i.contrasena == contrasena select i).FirstOrDefault();
                    if (usr != null)
                    {
                        string consulta =
                        "SELECT c.id_distrito_local FROM sice_casillas C where id_cabecera_local = " + usr.id_municipio + " " +
                        "GROUP BY C.id_distrito_local ";
                        LoginInfo.lista_distritos = contexto.Database.SqlQuery<int>(consulta).ToList();
                        LoginInfo.id_usuario = usr.id;
                        LoginInfo.nombre = usr.nombre;
                        LoginInfo.apellido = usr.apellido;
                        LoginInfo.id_municipio = usr.id_municipio;
                        LoginInfo.privilegios = usr.privilegios;
                        LoginInfo.id_cabecera_local = usr.id_municipio;
                        LoginInfo.nombre_formal = usr.nombre_formal;

                        if (usr.privilegios == 5 || usr.privilegios == 4)
                            return 2;
                        return 1;
                    }

                    return 0;
                }


            }
            catch (Exception ex)
            {
                //string innerEx = ex.InnerException.Message;
                //if (innerEx == "Unable to connect to any of the specified MySQL hosts.")
                //{
                //    return 2;
                //}
                //else
                //{
                //    return 3;
                //}
                return 3;


            }

        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
