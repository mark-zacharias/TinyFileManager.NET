﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TinyFileManager.NET
{
    public class DirectorySource
    {
        private string[] arrFolders;
        private string[] arrFiles;
        private string strCurrPath;
        private string strCurrLink;
        private bool boolOnlyImage;
        private bool boolOnlyVideo;
        private ArrayList arrLinks = new ArrayList();
        private string physicalPath;
        private string strApply;
        private string strType;

        public DirectorySource(string currentPath, string currentLink, bool onlyImages, bool onlyVideos, string physicalPath, string selectFnString, string type)
        {
            strCurrPath = currentPath;
            strCurrLink = currentLink;
            boolOnlyImage = onlyImages;
            boolOnlyVideo = onlyVideos;
            this.physicalPath = physicalPath;
            strApply = selectFnString;
            strType = type;

            AddFolders();
            AddFiles();
        }

        public ArrayList GetLinks()
        {
            return arrLinks;
        }

        private void AddFiles()
        {
            // load files
            arrFiles = Directory.GetFiles(clsConfig.strUploadPath + this.strCurrPath);
            foreach (string strF in arrFiles)
            {
                var objFItem = new TinyFileManager.NET.clsFileItem();
                objFItem.strName = Path.GetFileNameWithoutExtension(strF);
                objFItem.boolIsFolder = false;
                //objFItem.intColNum = this.getNextColNum();
                objFItem.strPath = this.strCurrPath + Path.GetFileName(strF);
                objFItem.boolIsImage = Helper.isImageFile(Path.GetFileName(strF));
                objFItem.boolIsVideo = Helper.isVideoFile(Path.GetFileName(strF));
                objFItem.boolIsMusic = Helper.isMusicFile(Path.GetFileName(strF));
                objFItem.boolIsMisc = Helper.isMiscFile(Path.GetFileName(strF));
                // get display class type
                if (objFItem.boolIsImage)
                {
                    objFItem.strClassType = "2";
                }
                else
                {
                    if (objFItem.boolIsMisc)
                    {
                        objFItem.strClassType = "3";
                    }
                    else
                    {
                        if (objFItem.boolIsMusic)
                        {
                            objFItem.strClassType = "5";
                        }
                        else
                        {
                            if (objFItem.boolIsVideo)
                            {
                                objFItem.strClassType = "4";
                            }
                            else
                            {
                                objFItem.strClassType = "1";
                            }
                        }
                    }
                }
                // get delete link
                if (clsConfig.boolAllowDeleteFile)
                {
                    objFItem.strDeleteLink = "<a href=\"" + this.strCurrLink + "&cmd=delfile&file=" + objFItem.strPath + "&currpath=" + this.strCurrPath + "\" class=\"btn erase-button\" onclick=\"return confirm('Are you sure to delete this file?');\" title=\"Erase\"><i class=\"icon-trash\"></i></a>";
                }
                else
                {
                    objFItem.strDeleteLink = "<a class=\"btn erase-button disabled\" title=\"Erase\"><i class=\"icon-trash\"></i></a>";
                }
                // get thumbnail image
                if (objFItem.boolIsImage)
                {
                    objFItem.strThumbImage = clsConfig.strThumbURL + "/" + objFItem.strPath.Replace('\\', '/');
                }
                else
                {
                    if (File.Exists(Directory.GetParent(physicalPath).FullName + "\\img\\ico\\" + Path.GetExtension(strF).TrimStart('.').ToUpper() + ".png"))
                    {
                        objFItem.strThumbImage = "img/ico/" + Path.GetExtension(strF).TrimStart('.').ToUpper() + ".png";
                    }
                    else
                    {
                        objFItem.strThumbImage = "img/ico/Default.png";
                    }
                }
                objFItem.strDownFormOpen = "<form action=\"dialog.aspx?cmd=download&file=" + objFItem.strPath + "\" method=\"post\" class=\"download-form\">";
                objFItem.strDownBtn = "<button type=\"submit\" title=\"Download\" class=\"btn\"><i class=\"icon-download\"></i></button>";
                if (objFItem.boolIsImage)
                {
                    objFItem.strPreviewLink = "<a class=\"btn preview\" title=\"Preview\" data-url=\"" + clsConfig.strUploadURL + "/" + objFItem.strPath.Replace('\\', '/') + "\" data-toggle=\"lightbox\" href=\"#previewLightbox\"><i class=\"icon-eye-open\"></i></a>";
                }
                else
                {
                    objFItem.strPreviewLink = "<a class=\"btn preview disabled\" title=\"Preview\"><i class=\"icon-eye-open\"></i></a>";
                }
                objFItem.strLink = "<a href=\"#\" title=\"Select\" onclick=\"" + this.strApply + "('" + clsConfig.strUploadURL + "/" + objFItem.strPath.Replace('\\', '/') + "'," + this.strType + ")\";\"><img data-src=\"holder.js/140x100\" alt=\"140x100\" src=\"" + objFItem.strThumbImage + "\" height=\"100\"><h4>" + objFItem.strName + "</h4></a>";

                // check to see if it's the type of file we are looking at
                if ((this.boolOnlyImage && objFItem.boolIsImage) || (this.boolOnlyVideo && objFItem.boolIsVideo) || (!this.boolOnlyImage && !this.boolOnlyVideo))
                {
                    this.arrLinks.Add(objFItem);
                }
            }
        }

        private void AddFolders()
        {

            //load folders
            arrFolders = Directory.GetDirectories(clsConfig.strUploadPath + this.strCurrPath);
            foreach (string strF in arrFolders)
            {
                var objFItem = new TinyFileManager.NET.clsFileItem();
                objFItem.strName = Path.GetFileName(strF);
                objFItem.boolIsFolder = true;
                //objFItem.intColNum = this.getNextColNum();
                objFItem.strPath = this.strCurrPath + Path.GetFileName(strF);
                objFItem.strClassType = "dir";
                if (clsConfig.boolAllowDeleteFolder)
                {
                    objFItem.strDeleteLink = "<a href=\"" + this.strCurrLink + "&cmd=delfolder&folder=" + objFItem.strPath + "&currpath=" + this.strCurrPath + "\" class=\"btn erase-button top-right\" onclick=\"return confirm('Are you sure to delete the folder and all the objects in it?');\" title=\"Erase\"><i class=\"icon-trash\"></i></a>";
                }
                else
                {
                    objFItem.strDeleteLink = "<a class=\"btn erase-button top-right disabled\" title=\"Erase\"><i class=\"icon-trash\"></i></a>";
                }
                objFItem.strThumbImage = "img/ico/folder.png";
                objFItem.strLink = "<a title=\"Open\" href=\"" + this.strCurrLink + "&currpath=" + objFItem.strPath + "\"><img class=\"directory-img\" src=\"" + objFItem.strThumbImage + "\" alt=\"folder\" /><h3>" + objFItem.strName + "</h3></a>";
                this.arrLinks.Add(objFItem);
            }
        }
    }
}