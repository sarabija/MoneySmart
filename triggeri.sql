use moneysmart;


DELIMITER $$

CREATE TRIGGER after_revenue_insert
AFTER INSERT ON revenue
FOR EACH ROW
BEGIN
    IF NEW.revenueMode = 'Cash' OR NEW.revenueMode = 'Gotovina' THEN
        UPDATE balance
        SET cash = cash + NEW.revenueValue
        WHERE balanceID = NEW.balance_balanceID;
    ELSEIF NEW.revenueMode = 'Credit card' OR NEW.revenueMode = 'Kartica' THEN
        UPDATE balance
        SET credit = credit + NEW.revenueValue
        WHERE balanceID = NEW.balance_balanceID;
    END IF;
END$$

DELIMITER ;
DELIMITER $$

CREATE TRIGGER after_spending_insert
AFTER INSERT ON spending
FOR EACH ROW
BEGIN
    IF NEW.spendingType = 'Cash' OR NEW.spendingType = 'Gotovina' THEN
        UPDATE balance
        SET cash = cash - NEW.spendingValue
        WHERE balanceID = NEW.balance_balanceID;
    ELSEIF NEW.spendingType = 'Credit card' OR NEW.spendingType = 'Kartica' THEN
        UPDATE balance
        SET credit = credit - NEW.spendingValue
        WHERE balanceID = NEW.balance_balanceID;
    END IF;
END$$

DELIMITER ;


DELIMITER $$

CREATE TRIGGER after_revenue_update
AFTER UPDATE ON revenue
FOR EACH ROW
BEGIN
    IF NEW.revenueMode = 'Cash' OR NEW.revenueMode = 'Gotovina' THEN
        UPDATE balance
        SET cash = cash + NEW.revenueValue - OLD.revenueValue
        WHERE balanceID = NEW.balance_balanceID;
    ELSEIF NEW.revenueMode = 'Credit card' OR NEW.revenueMode = 'Kartica' THEN
        UPDATE balance
        SET credit = credit + NEW.revenueValue - OLD.revenueValue
        WHERE balanceID = NEW.balance_balanceID;
    END IF;
END$$

DELIMITER ;

DELIMITER $$

CREATE TRIGGER after_spending_update
AFTER UPDATE ON spending
FOR EACH ROW
BEGIN
    IF NEW.spendingType = 'Cash' OR NEW.spendingType = 'Gotovina' THEN
        UPDATE balance
        SET cash = cash - NEW.spendingValue + OLD.spendingValue
        WHERE balanceID = NEW.balance_balanceID;
    ELSEIF NEW.spendingType = 'Credit card' OR NEW.spendingType = 'Kartica' THEN
        UPDATE balance
        SET credit = credit - NEW.spendingValue + OLD.spendingValue
        WHERE balanceID = NEW.balance_balanceID;
    END IF;
END$$

DELIMITER ;

DELIMITER $$

CREATE TRIGGER after_saving_update
AFTER UPDATE ON saving
FOR EACH ROW
BEGIN
    IF NEW.savingDeposit <> OLD.savingDeposit THEN
        UPDATE balance
        SET cash = cash - NEW.savingDeposit + OLD.savingDeposit
        WHERE balanceID = NEW.balance_balanceID;
    END IF;
END$$

DELIMITER ;

DELIMITER $$

CREATE TRIGGER after_saving_insert
AFTER INSERT ON saving
FOR EACH ROW
BEGIN
    -- Deduct the new savingDeposit from the corresponding balance cash
    UPDATE balance
    SET cash = cash - NEW.savingDeposit
    WHERE balanceID = NEW.balance_balanceID;
END$$

DELIMITER ;


 SELECT MONTH(revenueDate) AS Month, 
       SUM(CASE 
               WHEN (revenueType = 'One-Time' OR revenueType = 'Jednokratni') 
               THEN revenueValue 
               ELSE 0 
            END) AS OneTimeRevenue, 
       SUM(CASE 
               WHEN (revenueType = 'Monthly' OR revenueType = 'Mesečni') 
               THEN revenueValue 
               ELSE 0 
            END) AS RegularRevenue,
       SUM(CASE 
               WHEN (revenueType = 'Annual' OR revenueType = 'Godišnji') 
               THEN revenueValue / 12 
               ELSE 0 
            END) AS AnnualAverageRevenue
FROM revenue
WHERE balance_balanceID = 2
  AND YEAR(revenueDate) = 2024
GROUP BY MONTH(revenueDate)
ORDER BY MONTH(revenueDate);