using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;

// very limited local cache class
// will make sure all files in a list of urls are on the local disk
// will throw away any other files in the cache folder
// can NOT handle files with same names

public class Cache {
                                //original    <CachedURL, thumbcached>
    protected readonly Dictionary<string, Tuple<string, string>> cachedUrls = new Dictionary<string, Tuple<string, string>>();
    private string thumbnailExt = ".thumb";
    private int thumbWidth = 500;
    public string GetCachedPath(string url) {
        return cachedUrls[url].Item1;
    }
    public string GetCachedThumbnailPath(string url)
    {
        return cachedUrls[url].Item2;
    }
    public bool HasCached(string url) {
        return cachedUrls.ContainsKey(url);
    }

    public bool errorWhileCacheOnly;

    // Central and only method to cache data! Needs to be called before other methods work.
    // deletes everything that is not in urls 
    // caches everything in urls
    public IEnumerator CacheOnly(IEnumerable<MyFile> newFiles) {

        errorWhileCacheOnly = false;

        List<string> urls = newFiles.Select(f => f.url).ToList();
        urls.AddRange( newFiles.Select(f => f.thumbnailURL));

        Directory.CreateDirectory(Paths.cachePath);

        var cf = Directory.GetFiles(Paths.cachePath);
        var cachedFiles = cf.Where(f => !f.EndsWith(thumbnailExt));

        var newFileNames = urls.Select(url => Path.GetFileName(url)); 

        foreach(var file in cachedFiles)    // compare all cached files with new ones
        {
            if (!newFileNames.Contains(Path.GetFileName(file)))
            { 
                DeleteFile(file);
            }
        }

        // re-find cached files
        cachedFiles = Directory.GetFiles(Paths.cachePath);

        foreach (var newfile in newFiles)
        {
            var matchingCachedFile = cachedFiles.FirstOrDefault(cached => Path.GetFileName(cached) ==  Path.GetFileName(newfile.url));
            var matchingCachedThumb = cachedFiles.FirstOrDefault(cached => Path.GetFileName(cached) == Path.GetFileName(newfile.url) + thumbnailExt);
            
            if(matchingCachedFile != null)                                  
            {
                cachedUrls[newfile.url] = Tuple.Create(matchingCachedFile, "");

                if (matchingCachedThumb != null)                            // found this file and thumb cached
                    cachedUrls[newfile.url] = Tuple.Create(matchingCachedFile, matchingCachedThumb);
                else                                                        // found file without thumb
                    yield return CreateCachedThumbnail(newfile);
            }
            else if(matchingCachedThumb != null)                            // found orphaned thumbb to this file.. should not happen lol but reload 
            {
                yield return DoCache(newfile);
                cachedUrls[newfile.url] = Tuple.Create(cachedUrls[newfile.url].Item1, matchingCachedThumb);
            }
            else                                                            // file not found -> reload to cache
            {
                yield return DoCache(newfile);
                if(!errorWhileCacheOnly)
                    yield return CreateCachedThumbnail(newfile);
            }
        }
    }

    IEnumerator DoCache(MyFile f) {

        var fileName = Path.GetFileName(f.url);
        var thumbName = Path.GetFileName(f.thumbnailURL);

        var localPath = Path.Combine(Paths.cachePath, fileName);
        var localPath_thumb = localPath + thumbnailExt;

        using (UnityWebRequest req = UnityWebRequest.Get(new Uri(f.url))) {

            req.timeout = 500; // TODO either make parametric or track progress
            req.downloadHandler = new DownloadHandlerFile(localPath);
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
                errorWhileCacheOnly = true;
                yield break;
            }

            cachedUrls[f.url] = Tuple.Create(localPath, "");
        }
    }
    
    private void DeleteFile(string file)
    {
        if (File.Exists(file))
            File.Delete(file);
        else Debug.Log("Tried to delete file that does not exist. This must be a mistake in the cache.");

        if (File.Exists(file + thumbnailExt))   // delete also the cached thumbnail
            File.Delete(file + thumbnailExt);
    }

    public void DeleteCache()
    {
        foreach (var file in Directory.GetFiles(Paths.cachePath))
            File.Delete(file);
    }

    private IEnumerator CreateCachedThumbnail(MyFile f)
    {
        switch (f)
        {
            case  ImageFile i:
                GenerateThumbNailforImage(i);
                break;
            case VideoFile v:
                if (!string.IsNullOrWhiteSpace(f.thumbnailURL))
                    yield return GenerateThumbNailforVideo(v);
                break;
            default:
                break;
        }
    }

    private void GenerateThumbNailforImage(ImageFile f)
    {
        var localUrl = cachedUrls[f.url].Item1;
        var localThumbUrl = localUrl + thumbnailExt;

        var data = File.ReadAllBytes(localUrl);    // hmm reload local cached image.. not so nice?

        WriteScaledImage(data, localThumbUrl);
        
        cachedUrls[f.url] = Tuple.Create(localUrl, localThumbUrl);     // fill cache with correct urls
    }
    private IEnumerator GenerateThumbNailforVideo(VideoFile f)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(new Uri(f.thumbnailURL)))
        {
            var localUrl = cachedUrls[f.url].Item1;
            var localThumbUrl = localUrl + thumbnailExt;

            req.timeout = 500; // TODO either make parametric or track progress
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
                errorWhileCacheOnly = true;
                yield break;
            }

            try
            {
                WriteScaledImage(req.downloadHandler.data, localThumbUrl);

                cachedUrls[f.url] = Tuple.Create(localUrl, localThumbUrl); 
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                errorWhileCacheOnly = true;
                yield break;
            }
        }
    }
    //Texture2D tempTex;  // little hacky - to avoid 'new Texture' to battle memory leak
    private void WriteScaledImage(byte[] data, string localThumbUrl)
    {
        var tempTex = new Texture2D(2, 2);        // texture size does not matter -> loadimage resizes
        tempTex.hideFlags = HideFlags.HideAndDontSave;
        
        ImageConversion.LoadImage(tempTex, data);
        var newHeight = thumbWidth / ((float)tempTex.width / (float)tempTex.height);
        TextureScale.Bilinear(tempTex, (int)thumbWidth, (int)(newHeight));

        File.WriteAllBytes(localThumbUrl, tempTex.EncodeToPNG());
        UnityEngine.Object.Destroy(tempTex);
    }
}