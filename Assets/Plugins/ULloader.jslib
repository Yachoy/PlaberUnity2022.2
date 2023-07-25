mergeInto(LibraryManager.library,
    {
        InitFileLoader: function (callbackObjectName, callbackMethodName) {
						// Полученные из C# строки необходимо декодировать из UTF8
            FileCallbackObjectName = UTF8ToString(callbackObjectName);
            FileCallbackMethodName = UTF8ToString(callbackMethodName);
						
          	// Создаем input для взятия файлов, если такого еще нет
            var fileuploader = document.getElementById('fileuploader');
            if (!fileuploader) {
                console.log('Creating fileuploader...');
                fileuploader = document.createElement('input');
                fileuploader.setAttribute('style', 'display:none;');
                fileuploader.setAttribute('type', 'file');
                fileuploader.setAttribute('id', 'fileuploader');
                fileuploader.setAttribute('class', 'nonfocused');
                document.getElementsByTagName('body')[0].appendChild(fileuploader);

                fileuploader.onchange = function (e) {
                    var files = e.target.files;
										
                  	// Если файл не выбран - завершаем выполнение и вызываем unfocus
                  	// Пометка: Если необходимо обрабатывать случай, когда файл не
                  	// выбран, то тут можно вызывать SendMessage и передавать ему
                  	// null, вместо ResetFileLoader()
										if (files.length === 0) {
                        ResetFileLoader();
                        return;
                    }
                  
                    console.log('ObjectName: ' + FileCallbackObjectName + ';\nMethodName: ' + FileCallbackMethodName + ';');
                    SendMessage(FileCallbackObjectName, FileCallbackMethodName, URL.createObjectURL(files[0]));
                };
            }

            console.log('FileLoader initialized!');
        },


				// Эта функция вызывается на нажатие кнопки, т.к. защита браузера не пропускает вызов click()
  			// программно
        RequestUserFile: function (extensions) {
          	// Переводим строку из UTF8
            var str = UTF8ToString(extensions);
            var fileuploader = document.getElementById('fileuploader');
						
          	// Если по каким-то причинам fileuploader не существует - задаем его
          	// Это может случиться в проектах Blazor.NET
            if (fileuploader === null)
                InitFileLoader(FileCallbackObjectName, FileCallbackMethodName);
						
          	// Задаем полученные расширения
            if (str !== null || str.match(/^ *$/) === null)
                fileuploader.setAttribute('accept', str);
						
          	// Фокус на инпут и клик
            fileuploader.setAttribute('class', 'focused');
            fileuploader.click();
        },
				
  			// Эта функция вызывается после получения файла
  			// Её можно вызывать из RequestUserFile или fileUploader.onchange
  			// а не из C#, что будет быстрее, но я использую вызов из C# как мини-пример
  			// вызова функции без параметров
        ResetFileLoader: function () {
            var fileuploader = document.getElementById('fileuploader');

            if (fileuploader) {
              	// Убираем инпут из фокуса
                fileuploader.setAttribute('class', 'nonfocused');
            }
        },
    });