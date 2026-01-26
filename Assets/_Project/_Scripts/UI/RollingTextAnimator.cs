using UnityEngine;
using TMPro;
using System.Collections;
using NaughtyAttributes;

public class RollingTextAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentText;    // Ссылка на текущий TextMeshProUGUI
    [SerializeField] private float _stepDuration = 0.3f;      // Время (секунд) на прокрутку между двумя соседними значениями
    
    private Coroutine animationCoroutine;
    private int? currentNumber;            // Текущее число (null, если сейчас отображается не число, а текст)
    private int? targetNumber;             // Целевое число для анимации (если используется ChangeNumber)

    public void ChangeTextQuick(string newText)
    {
        // Если сейчас выполняется анимация – прерываем её для замены текста
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        // Просто меняем текст на новый без анимации
        _currentText.text = newText;
        currentNumber = null;
        targetNumber = null;
    }
    
    // Метод для смены текста на новый текст (без промежуточных значений)
    public void ChangeText(string newText, bool directionUp)
    {
        // Если сейчас выполняется анимация – прерываем её для замены текста
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        // Запускаем анимацию смены текста
        animationCoroutine = StartCoroutine(AnimateTextChange(newText, directionUp));
    }

    public void ChangeNumberQuick(int newNumber)
    {
        // Если сейчас выполняется анимация – прерываем её для замены числа
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        // Просто меняем текст на новое число без анимации
        _currentText.text = newNumber.ToString();
        currentNumber = newNumber;
        targetNumber = null;
    }
    
    // Метод для смены текущего числа на новое с перебором промежуточных значений
    public void ChangeNumber(int newNumber)
    {
        // Если уже идёт анимация, просто обновляем цель – корутина подхватит новое значение
        targetNumber = newNumber;
        if (animationCoroutine == null)
        {
            // Если анимация не запущена, запускаем новую
            animationCoroutine = StartCoroutine(AnimateNumberChange(newNumber));
        }
        // Если анимация уже идёт, новое значение `targetNumber` будет учтено на следующем шаге
    }

    // Корутина плавной смены произвольного текста на новый
    private IEnumerator AnimateTextChange(string newText, bool directionUp)
    {
        // Создаём копию текстового объекта для нового текста
        TextMeshProUGUI nextText = Instantiate(_currentText, _currentText.transform.parent);
        nextText.name = "RollingText";
        nextText.text = newText;
        // Расположим новый текст выше или ниже видимой области
        float height = _currentText.rectTransform.rect.height;
        if (directionUp)
        {
            // Новый текст появляется снизу (т.к. старый уходит вверх)
            nextText.rectTransform.anchoredPosition = new Vector2(0, -height);
        }
        else
        {
            // Новый текст появляется сверху (старый уходит вниз)
            nextText.rectTransform.anchoredPosition = new Vector2(0, height);
        }

        // Вычисляем конечные позиции для старого и нового текстов
        Vector2 targetPosCurrent = directionUp 
                                   ? new Vector2(0, height)    // уезжает вверх
                                   : new Vector2(0, -height);  // уезжает вниз
        Vector2 targetPosNext = Vector2.zero; // центр контейнера (0,0)

        // Выполняем плавное перемещение в течение stepDuration секунд
        float elapsed = 0f;
        Vector2 startPosCurrent = _currentText.rectTransform.anchoredPosition;
        Vector2 startPosNext = nextText.rectTransform.anchoredPosition;
        while (elapsed < _stepDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _stepDuration);
            // Линейная интерполяция позиций (можно добавить сглаживание по желанию)
            _currentText.rectTransform.anchoredPosition = Vector2.Lerp(startPosCurrent, targetPosCurrent, t);
            nextText.rectTransform.anchoredPosition = Vector2.Lerp(startPosNext, targetPosNext, t);
            yield return null;
        }
        // Гарантируем, что достигли конечного положения
        _currentText.rectTransform.anchoredPosition = targetPosCurrent;
        nextText.rectTransform.anchoredPosition = targetPosNext;

        // Удаляем старый текст и назначаем новый как currentText
        Destroy(_currentText.gameObject);
        _currentText = nextText;
        // Обновляем текущее значение (число сбрасываем, так как это могла быть строка)
        currentNumber = null;

        // Анимация завершена
        animationCoroutine = null;
    }

    // Корутина плавной смены числа с перебором всех промежуточных значений
    private IEnumerator AnimateNumberChange(int initialTarget)
    {
        // Если currentNumber ещё не установлен, попробуем получить его из currentText (например, если там число)
        if (currentNumber == null)
        {
            int parsed;
            if (int.TryParse(_currentText.text, out parsed))
                currentNumber = parsed;
            else
                currentNumber = 0; // Если парсинг не удался, считаем что текущее число 0
        }

        // Устанавливаем начальную и конечную цель
        int startValue = currentNumber.Value;
        targetNumber = initialTarget;

        while (currentNumber.Value != targetNumber.Value)
        {
            // Определяем направление шага (+1 или -1)
            int step = (targetNumber.Value > currentNumber.Value) ? 1 : -1;
            int nextValue = currentNumber.Value + step;
            string nextTextValue = nextValue.ToString();

            // Запускаем анимацию смены currentNumber -> nextValue (переиспользуем AnimateTextChange)
            // Направление движения: если увеличиваем (step=1) -> directionUp = true? (см. пояснение ниже)
            // Здесь directionUp = false, если увеличиваем (новое число больше) – старый уходит вниз, новый спускается сверху.
            bool directionUp = step < 0; // если уменьшаем, то true (уход вверх), если увеличиваем, то false (уход вниз)
            // Создаём новый текст и анимируем переход
            TextMeshProUGUI nextText = Instantiate(_currentText, _currentText.transform.parent);
            nextText.name = "RollingText_" + nextTextValue;
            nextText.text = nextTextValue;
            float height = _currentText.rectTransform.rect.height;
            if (directionUp)
                nextText.rectTransform.anchoredPosition = new Vector2(0, -height);
            else
                nextText.rectTransform.anchoredPosition = new Vector2(0, height);

            Vector2 targetPosCurrent = directionUp ? new Vector2(0, height) : new Vector2(0, -height);
            Vector2 targetPosNext = Vector2.zero;
            float elapsed = 0f;
            Vector2 startPosCurrent = _currentText.rectTransform.anchoredPosition;
            Vector2 startPosNext = nextText.rectTransform.anchoredPosition;
            while (elapsed < _stepDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _stepDuration);
                _currentText.rectTransform.anchoredPosition = Vector2.Lerp(startPosCurrent, targetPosCurrent, t);
                nextText.rectTransform.anchoredPosition = Vector2.Lerp(startPosNext, targetPosNext, t);
                yield return null;
            }
            // Завершаем движение
            _currentText.rectTransform.anchoredPosition = targetPosCurrent;
            nextText.rectTransform.anchoredPosition = targetPosNext;
            // Удаляем старый объект текста
            Destroy(_currentText.gameObject);
            // Обновляем currentText и текущее значение
            _currentText = nextText;
            currentNumber = nextValue;

            // Проверяем, не изменилась ли цель во время текущего шага
            if (targetNumber.HasValue && targetNumber.Value != initialTarget)
            {
                // Цель изменилась вне этой корутины (поступил вызов ChangeNumber),
                // обновляем initialTarget и продолжаем цикл с новым targetNumber
                initialTarget = targetNumber.Value;
            }
        }

        // Анимация завершена
        animationCoroutine = null;
    }

    #region Debug

    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void SwitchToZero()
    {
        ChangeNumber(0);
    }

    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void SwitchTo10()
    {
        ChangeNumber(10);
    }
    
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void SwitchToQuestion()
    {
        ChangeText("?", true);
    }
    
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void SwitchToPlus2()
    {
        ChangeText("+2", false);
    }
    
    #endregion
}
