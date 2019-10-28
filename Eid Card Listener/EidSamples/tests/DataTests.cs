/* ****************************************************************************

 * eID Middleware Project.
 * Copyright (C) 2010-2016 FedICT.
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License version
 * 3.0 as published by the Free Software Foundation.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, see
 * http://www.gnu.org/licenses/.

**************************************************************************** */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EidSamples;
using System.Collections.Generic;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace EidSamples.tests
{
    /// <summary> 
    /// Tests some basic data retrieval (from the eID card, or the pkcs11 module)
    /// </summary>
    [TestClass]
    public class DataTests
    {
        /// <summary>
        /// Tests if pkcs11 finds the attached card reader "ACS CCID USB Reader 0"
        /// Test is only valid if such card reader is attached
        /// </summary>
        [TestMethod]
        public void GetSlotDescription()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            Assert.AreEqual("ACS CCID USB Reader 0", dataTest.GetSlotDescription());
        }
        /// <summary>
        /// Tests if pkcs11 labels the eID card as "BELPIC"
        /// </summary>
        [TestMethod]
        public void GetTokenInfoLabel()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            Assert.AreEqual("BELPIC", dataTest.GetTokenInfoLabel().Trim());
        }
        /// <summary>
        /// Tests the retrieval of the special status from the parsed identity file from the eID card
        /// </summary>
        [TestMethod]
        public void GetSpecialStatus()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            dataTest.GetSpecialStatus();
            //Assert.AreEqual("SPECIMEN", dataTest.GetSurname());
        }
        /// <summary>
        /// Tests the retrieval of the surname from the parsed identity file from the eID card
        /// </summary>
        [TestMethod]
        public void GetSurname()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            dataTest.GetSurname();
            //Assert.AreEqual("SPECIMEN", dataTest.GetSurname());
        }
        public string MyDictionaryToJson(Dictionary<string, string> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\":\"{1}\"", d.Key, string.Join(",", d.Value)));
            return "{" + string.Join(",\n", entries) + "}\n";}

        public Dictionary<string,string> GetAllData()

        {
            string[] labelTab = new string[24];
            labelTab[0] = "card_number";
            labelTab[1] = "chip_number";
            labelTab[2] = "validity_begin_date";
            labelTab[3] = "validity_end_date";
            labelTab[4] = "issuing_municipality";
            labelTab[5] = "national_number";
            labelTab[6] = "surname";
            labelTab[7] = "firstnames";
            labelTab[8] = "first_letter_of_third_given_name";
            labelTab[9] = "nationality";
            labelTab[10] = "location_of_birth";
            labelTab[11] = "date_of_birth";
            labelTab[12] = "gender";
            labelTab[13] = "nobility";
            labelTab[14] = "document_type";
            labelTab[15] = "special_status";
            labelTab[16] = "special_organization";
            labelTab[17] = "duplicata";
            labelTab[18] = "member_of_family";
            labelTab[19] = "photo_hash";
            labelTab[20] = "date_and_country_of_protection";
            labelTab[21] = "address_street_and_number";
            labelTab[22] = "address_zip";
            labelTab[23] = "address_municipality";

            ReadData dataTest = new ReadData("beidpkcs11.dll");
            try
            {
                Dictionary<string,string> map = dataTest.GetDataAll(labelTab);      
                System.IO.File.WriteAllText(@"" + Program.path+"\\"+ "eid-" + map["national_number"] + ".json", "{\"id_data\":" + MyDictionaryToJson(map)+"}");

                return map;
            }
            catch(Exception e) { Console.WriteLine(e.ToString());

                return new Dictionary<string, string>();
            }
        }

        public Dictionary<string, byte[]> GetAllDataByte()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            return dataTest.GetAllDataByte();
            //Assert.AreEqual("SPECIMEN", dataTest.GetSurname());
        }

        private string run_cmd(string pythonPath, string scriptPath, string niss, string dateIn, string dateOut, string mut, string aff = "", string dateAff = "")

        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = pythonPath;
                if (aff == "")
                    start.Arguments = string.Format("{0} {1} {2} {3} {4} ", scriptPath, niss, dateIn, dateOut, mut);
                else if (dateAff == "")
                    start.Arguments = string.Format("{0} {1} {2} {3} {4} {5}", scriptPath, niss, dateIn, dateOut, mut, aff);
                else
                    start.Arguments = string.Format("{0} {1} {2} {3} {4} {5} {6}", scriptPath, niss, dateIn, dateOut, mut, aff, dateAff);
                start.UseShellExecute = false;
                start.CreateNoWindow = true; // Hide the command line window
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;

                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch(Exception e) { return e.StackTrace; }
        }

        private string start(string niss)

        {

            string scriptPath = "pythonScript.py";
            string pythonPath = @"Python\python.exe";
            return run_cmd(pythonPath, scriptPath, niss, "01012019", "01012019", "all");
            
        }

        /// <summary>
        /// Tests the retrieval of the birth date from the parsed identity file from the eID card
        /// </summary>
        [TestMethod]
        public void GetDateOfBirth()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            dataTest.GetDateOfBirth();
            //Assert.AreEqual("01 JAN 1971", dataTest.GetDateOfBirth());
        }
        /// <summary>
        /// Tests the retrieval of the Identity file from the eID card
        /// </summary>
        [TestMethod]
        public void GetIdFile()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            byte [] idFile = dataTest.GetIdFile();
            int i = 0;
            
            // poor man's tlv parser...
            // we'll check the first two tag fields (01 and 02)
            Assert.AreEqual(0x01, idFile[i++]); // Tag
            i += idFile[i];                     // Length - skip value
            i++;
            Assert.AreEqual(0x02, idFile[i]); // Tag
        }
        /// <summary> 
        /// Tests the retrieval of the Authentication certificate label
        /// This test is only valid for eID cards with an authentication certificate
        /// </summary> 
        [TestMethod]
        public void GetCertificateLabels()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            List<string> labels = dataTest.GetCertificateLabels();
            Assert.IsTrue(labels.Contains("Authentication"),"Find Authentication certificate");
        }
        /// <summary> 
        /// Tests the retrieval of the RN certificate, and check if it is named 'root'
        /// </summary> 
        [TestMethod]
        public void GetCertificateRNFile()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            byte[] certificateRNFile = dataTest.GetCertificateRNFile();
            X509Certificate certificateRN;
            try
            {
                certificateRN = new X509Certificate(certificateRNFile);
                Assert.IsTrue(certificateRN.Issuer.Contains("Root"));
            }
            catch
            {
                Assert.Fail();
            }   
        }
        /// <summary> 
        /// Tests the retrieval of the Belgium root certificate, and check if it is self-signed, and named 'root'
        /// </summary> 
        [TestMethod]
        public void GetCertificateRootFile()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            byte[] certificateFile = dataTest.GetCertificateRootFile();
            X509Certificate certificateRoot;
            try
            {
                certificateRoot = new X509Certificate(certificateFile);
                Assert.IsTrue(certificateRoot.Subject.Contains("Root"));
                Assert.AreEqual(certificateRoot.Subject, certificateRoot.Issuer, "Should be a self-signed root certificate");
            }
            catch
            {
                Assert.Fail();
            }             
        }
        /// <summary> 
        /// Tests retrieval of the photo file, and checks if its size is as expected
        /// </summary> 
        [TestMethod]
        public void GetPhotoFile()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            byte[] photoFile = dataTest.GetPhotoFile();
            Bitmap photo = new Bitmap(new MemoryStream(photoFile));
            Assert.AreEqual(140, photo.Width);
            Assert.AreEqual(200, photo.Height);
            
        }
        /// <summary> 
        /// Tests the retrieval of the RN certificate, and try to add it in the my store
        /// </summary> 
        [TestMethod]
        public void StoreCertificateRNFile()
        {
            ReadData dataTest = new ReadData("beidpkcs11.dll");
            byte[] certificateRNFile = dataTest.GetCertificateRNFile();
            X509Certificate2 certificateRN = new X509Certificate2(certificateRNFile);
            
            X509Store mystore = new X509Store(StoreName.My);
            mystore.Open(OpenFlags.ReadWrite);
            mystore.Add(certificateRN);
        }
    }

}

