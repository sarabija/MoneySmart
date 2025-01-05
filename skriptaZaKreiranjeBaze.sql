-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema moneysmart
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema moneysmart
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `moneysmart` DEFAULT CHARACTER SET utf8 ;
USE `moneysmart` ;

-- -----------------------------------------------------
-- Table `moneysmart`.`balance`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `moneysmart`.`balance` (
  `balanceID` INT NOT NULL AUTO_INCREMENT,
  `cash` DECIMAL NULL,
  `credit` DECIMAL NULL,
  PRIMARY KEY (`balanceID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `moneysmart`.`revenue`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `moneysmart`.`revenue` (
  `revenueID` INT NOT NULL AUTO_INCREMENT,
  `revenueName` VARCHAR(45) NOT NULL,
  `revenueValue` DECIMAL NOT NULL,
  `revenueType` VARCHAR(45) NOT NULL,
  `revenueMode` VARCHAR(45) NULL,
  `balance_balanceID` INT NOT NULL,
  PRIMARY KEY (`revenueID`),
  INDEX `fk_revenue_balance1_idx` (`balance_balanceID` ASC) VISIBLE,
  CONSTRAINT `fk_revenue_balance1`
    FOREIGN KEY (`balance_balanceID`)
    REFERENCES `moneysmart`.`balance` (`balanceID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `moneysmart`.`user`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `moneysmart`.`user` (
  `userID` INT NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(100) NOT NULL,
  `password` VARCHAR(100) NOT NULL,
  `balance_balanceID` INT NOT NULL,
  `theme` VARCHAR(45) NOT NULL,
  `language` VARCHAR(45) NOT NULL,
  `expertUser` TINYINT NULL,
  PRIMARY KEY (`userID`),
  INDEX `fk_user_balance1_idx` (`balance_balanceID` ASC) VISIBLE,
  UNIQUE INDEX `expertUser_UNIQUE` (`expertUser` ASC) VISIBLE,
  CONSTRAINT `fk_user_balance1`
    FOREIGN KEY (`balance_balanceID`)
    REFERENCES `moneysmart`.`balance` (`balanceID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `moneysmart`.`spending`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `moneysmart`.`spending` (
  `spendingID` INT NOT NULL AUTO_INCREMENT,
  `spendingName` VARCHAR(45) NOT NULL,
  `spendingCategory` VARCHAR(45) NOT NULL,
  `spendingValue` DECIMAL NULL,
  `balance_balanceID` INT NOT NULL,
  PRIMARY KEY (`spendingID`),
  INDEX `fk_spending_balance1_idx` (`balance_balanceID` ASC) VISIBLE,
  CONSTRAINT `fk_spending_balance1`
    FOREIGN KEY (`balance_balanceID`)
    REFERENCES `moneysmart`.`balance` (`balanceID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `moneysmart`.`saving`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `moneysmart`.`saving` (
  `savingID` INT NOT NULL AUTO_INCREMENT,
  `savingName` VARCHAR(45) NOT NULL,
  `savingDeposit` DECIMAL NOT NULL,
  `savingGoal` DECIMAL NOT NULL,
  `balance_balanceID` INT NOT NULL,
  PRIMARY KEY (`savingID`),
  INDEX `fk_saving_balance1_idx` (`balance_balanceID` ASC) VISIBLE,
  CONSTRAINT `fk_saving_balance1`
    FOREIGN KEY (`balance_balanceID`)
    REFERENCES `moneysmart`.`balance` (`balanceID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `moneysmart`.`goal`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `moneysmart`.`goal` (
  `goalID` INT NOT NULL AUTO_INCREMENT,
  `goalName` VARCHAR(45) NOT NULL,
  `goalValue` DECIMAL NULL,
  `instalments` INT NULL,
  `balance_balanceID` INT NOT NULL,
  PRIMARY KEY (`goalID`),
  INDEX `fk_goal_balance1_idx` (`balance_balanceID` ASC) VISIBLE,
  CONSTRAINT `fk_goal_balance1`
    FOREIGN KEY (`balance_balanceID`)
    REFERENCES `moneysmart`.`balance` (`balanceID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `moneysmart`.`report`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `moneysmart`.`report` (
  `reportID` INT NOT NULL AUTO_INCREMENT,
  `reportName` VARCHAR(45) NOT NULL,
  `user_userID` INT NOT NULL,
  PRIMARY KEY (`reportID`),
  INDEX `fk_report_user1_idx` (`user_userID` ASC) VISIBLE,
  CONSTRAINT `fk_report_user1`
    FOREIGN KEY (`user_userID`)
    REFERENCES `moneysmart`.`user` (`userID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
