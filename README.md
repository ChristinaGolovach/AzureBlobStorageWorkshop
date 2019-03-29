## Task

#### Написать веб апи приложение для работы с изображениями. 

Приложение должно иметь два эндпоинта 
- Get по id для скачивания изображения **с применённым Bloor эффектом**
- Post для загрузки изображения 

При вызове метода Post изображение сохраняется в Blob Storage, далее добавляется сообщение в очередь Queue Storage. 
WebJob слушает очередь. При поступлении сообщения WebJob заменяет изображение в Blob Storage его копией с применённым Bloor эффектом. 

## Результат

#### Исходное изображение
![alt-текст](https://github.com/ChristinaGolovach/AzureBlobStorageWorkshop/blob/master/Screenshot_10.png "Исходное изображение")

#### Post endpont
![alt-текст](https://github.com/ChristinaGolovach/AzureBlobStorageWorkshop/blob/master/Screenshot_7.png "Post endpont")

#### Get(id) endpont
![alt-текст](https://github.com/ChristinaGolovach/AzureBlobStorageWorkshop/blob/master/Screenshot_5.png "Get(id) endpont")

#### Get endpoint
![alt-текст](https://github.com/ChristinaGolovach/AzureBlobStorageWorkshop/blob/master/Screenshot_4.png "Get endpoint")
