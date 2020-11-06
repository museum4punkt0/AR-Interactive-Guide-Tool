<?php
ini_set("display_errors","Yes");
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

if(empty($_POST)) die("No data given");
if(empty($_POST['password']) || $_POST['password'] !== "4VUZ#un7MA7CJWqC6EkKBDKs") die("Yeah no!");
if(empty($_POST['data']) || empty($_POST['filename'])) die("missing parameters");

//if(!file_exists("uploaded")) mkdir("uploaded");
// $filename = "uploaded/".$_POST['filename'].".json";
//if(!file_exists())
if(file_force_contents($_POST['filename'],$_POST['data']))
echo "Thank you!";

// https://gist.github.com/ofca-snippets/3730268
/**
 * Extended version of function file_put_contents.
 *
 * This function will create all dirs in the path to the file,
 * if they does not exists.
 *
 * @param   string  path to file
 * @param   string  file content
 * @param   int     flag taken by file_put_contents function
 * @return  bool    false on failure, true on success
 */
function file_force_contents($file_path, $content, $flag = LOCK_EX)
{
	// It's relative path, so we must change it to absolute
	//if ($file_path[0] != '/')
	//{
	//	$file_path = realpath($file_path);
	//}

	$dirname = pathinfo($file_path, PATHINFO_DIRNAME);

	// Create directories if not exists
	if ( ! file_exists($dirname))
	{
		// Create dirs
		mkdir($dirname, 0777, TRUE);

		// Set permissions (must be manually set to fix umask issues)
		chmod($dirname, 0777);
	}

	// Create file
	return (bool) file_put_contents($file_path, $content, $flag);
} // eo file_force_contents
?>